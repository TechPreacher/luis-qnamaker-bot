# luis-qnamaker-bot

## About
Microsoft Bot Framework bot that uses Luis.ai to detect the user's intent and if the intent score is low or the intent "none" is detected, hands off to QnAMaker.ai


## Prepare Luis.ai
Log in to Luis.ai and create a custom model.
Train and publish the LUIS.ai model and copy the URL presented to you by LUIS.ai on publishing. Note that **{APP_ID}**, **{SUBSCRIPTION_KEY}** and **{BING_KEY}** need to be replaced with the real values presented to you on the LUIS.ai publishing page:

    https://{AZURE_REGION}.api.cognitive.microsoft.com/luis/v2.0/apps/{APP_ID}?subscription-key={SUBSCRIPTION_KEY}&amp;spellCheck=true&amp;bing-spell-check-subscription-key={BING_KEY}&amp;verbose=true&amp;timezoneOffset=0&amp;q=

Copy it to the web.config file.

## Prepare QnAMaker.ai
Log in to QnAMaker.ai and create a new QnA Service.
On your "My QnA services" overview page (https://qnamaker.ai/Home/MyServices) get the Sample HTTP Request by clicking on Sample Code -> "View Code" beloning to the QnA service in question. find the **{qnaKnowledgebaseId}** and **{qnaSubscriptionKey}** and copy them to your project's web.config file.

    POST /knowledgebases/{qnaKnowledgebaseId}/generateAnswer
    Host: https://westus.api.cognitive.microsoft.com/qnamaker/v2.0
    Ocp-Apim-Subscription-Key: {qnaSubscriptionKey}
    Content-Type: application/json
    {"question":"hi"}


## Set up the bot
Log in to the [Azure Portal](http://portal.azure.com) and register a new **Bot Channels Registration**.
Copy down the **Micrsooft App ID** and **Microsoft App Password** and copy them to the **web.config** file of the main project.

    <add key="MicrosoftAppId" value="" />
    <add key="MicrosoftAppPassword" value="" />

## Publish the bot
Deploy the bot project as an Azure Web App and note it's endpoint in the following format:

    https://[yourname].azurewebsites.net/api/messages

Copy this endpoint to the bot framework configuration page's "Messaging Endpoint" field.

Test your bot in the interactive chat window in the bot framework page.