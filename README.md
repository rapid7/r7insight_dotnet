# Logging To InsightOps.

InsigtOps currently has plugins for NLog and Log4net.

To configure your application to log to InsightOps, you will need to perform the following tasks:

1. Create an Insight account.
2. Create a host and log to receive your log data.
3. Adding the Plugin library and the appropriate Insight Appender libraries to your application.
4. Configure the Plugin and the Insight appender in your application.
5. Send log messages from your application.

These steps are outlined in further detail below.

## Creating Your Insight Account

You can register your account on InsightOps by browsing to [https://www.rapid7.com/products/insightops/try/](https://www.rapid7.com/products/insightops/try/) and simply clicking `Start A Free Trial`.

## Creating the Host and Log

Once logged in to your InsightOps account, create a new host with a name that best represents your application. Select this host and create a new log with a source type of `Token TCP` (see below for more information) and a name that represents what you will be logging.

Please note that InsightOps reads no particular meaning into the names of your hosts and logs; they are primarily for your own benefit in keeping your logs organized.

## Links to Documentation

## NLog

Visit [NLog docs](https://insightops.help.rapid7.com/docs/nlog)

## Log4Net

Visit [Log4Net](https://insightops.help.rapid7.com/docs/log4net)
