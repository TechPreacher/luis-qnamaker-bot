using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using QnA_Maker_Test_Bot.Classes;

namespace QnA_Maker_Test_Bot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // Forward to LUIS
            Luis luisResult = await LuisClient.ParseUserInput(activity.Text);
            // Low scoring LUIS intent or None Intent
            if (luisResult.topScoringIntent.score < 1 || luisResult.intents[0].intent == "None") {
                // Forward to QnAMaker
                QnAMaker qnaMakerResult = await QnAMakerClient.ParseUserInput(activity.Text);
                // No answer found in QnAMaker
                if (qnaMakerResult.answers[0].qnaId == -1)
                {
                    await context.PostAsync("I don't have an answer for your question. Let me connect you to our support staff.");
                    // Hand off to real person.
                }
                // Low scoring QnAMaker answer
                else if(qnaMakerResult.answers[0].score < 0.5) {
                    foreach (Answer answerItem in qnaMakerResult.answers) {
                        await context.PostAsync("Possible Answer: " + answerItem.answer);
                    }
                    await context.PostAsync("Were any of these answers helpful?");
                    context.Wait(UserFeedbackReceivedAsync);
                    return;
                }
                // High scoring QnAMaker answer
                else {
                    await context.PostAsync("This likely answers your question: " + qnaMakerResult.answers[0].answer);
                }
            }
            // High scoring LUIS intent.
            else {
                await context.PostAsync("The following be able to help you solve your issue: " + luisResult.topScoringIntent.intent);
                // add custom answer based on LUIS intent.
            }
            context.Wait(MessageReceivedAsync);
        }

        private async Task UserFeedbackReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            if (activity.Text == "Yes") {
                await context.PostAsync("Thank you for your feedback. Can I help you with anything else?");
            }
            else {
                await context.PostAsync("Let me connect you to our support staff.");
                // Hand off to real person.
            }
            context.Wait(MessageReceivedAsync);
        }
    }
}