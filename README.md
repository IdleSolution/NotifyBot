# NotifyBot

The bot has the ability to subscribe to various sites and then notify you when someting new comes up. Sites it is currently supporting:

 - Mangadex - notifies you when a new chapter of a manga you are subscribed comes out (on the mangadex.org website);
 - Myanimelist - notifies you when a user you are subscribed to made a new post on MAL's forum

Supported commands:

 - !subscribe mangadex <link>
 - !subscribe mal <link>
 - !unsubscribe mangadex <link>
 - !unsubscribe mal <link>
 - !purge <amount>

If you want to use the bot by downloading it directly from github, you need to make your own files from Helpers/Paths (these txt files serve as a dummy database)

## NuGet packages the project is using:
* Discord.Net - version 2.1.1
* Discord.Net.Commands - version 2.1.1
* HtmlAgilityPack - version 1.11.12
