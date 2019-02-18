# ScreenshotSaverService
A prototype of a back-end service for inputting as list of URLs and receiving screenshots.

Requirements:
* The service should store the result of the request.
* The user would then be able to query the service for the results, the user should be able to
retrieve the results at any point in time.
* Message queues could be used to separate the different parts of the service and prepare
for scalability.


### Project uses in-memory application engine with a built in web server Starcounter (v2.4), that can be downloaded [here](https://downloads.starcounter.com/download)
Starcounter makes it possible to treat data really fast and is one of the optional solutions and steps while thinking about having 1000 000 screenshots requests per day. Another important step described in the comments is making IScreenShotMaker as an asynchronous service and introducing RabbitMq to deal with multiple requests in the queue
