#Sample collector

Imagine you are dealing with an application where data is constantly flowing through. However you would like to examine if this data is correct and valid so you would like to have a look at it. However sampling could be a exhausting process if done by hand. This project offers you a runtime solution to this problem. 

This project collects samples runtime with rates based on the provided configurations. When the criteria is met, the samples are "collected". In the current context this means that they're uploaded to the provided Azure BlobStorage. The structure of these files are kept in a specific hierarchy.

##How it works
First of all, when the client passes the options for the collector, an instance is created to check whether it already exists within the CacheMemory. 
If it does not exist, one is created. However, if the collector does exist, the settings just get passed to the next function.
Afterwards the application checks whether the collector has reached its limits. If not, the message gets processed by the algorithm which checks whether 
that specific object needs to be sampled(uploaded). This is determined by the percentage which is provided with the CollectorOptions. 

Keep in mind that it is possible to change the collector options at all times, even while your program is still in runtime. This works due to the fact that
the SampleCollector stores the settings within it's MemoryCache. This cache gets overridden every X hours/minutes/seconds which is decided by the CacheUpdater Interval.
If there is any change present, the options will get overridden. This means that this effect is triggered after the interval has passed. 
This is achieved by using the `BackgroundService`.

Because there is a MaxSamplesHour, MaxSamplesDay and MaxSamplesAlltime, we need to keep track of the number of samples in some way. 
This is done by MemoryCache in combination with the Database. At every end of an hour the data is getting synced to the database. 
Afterwards calculations are made to determine the number of samples. This is also achieved by using an `BackgroundService`.

If the algorithm determines that the object needs to be sampled(uploaded). It's getting passed to the StorageService which 
is responsible for uploading files to the Azure BlobStorage. To keep an order, it's decided to keep them in an specific hierarchy.
If a JSON file has been sampled on 01-12-2022 between 15:00 and 16:00 the file can be found inside the following path:
`~/ContainerName/CollectorName/Year/Month/Day/Hour/Filename.extension`

so the filepath of the example above will be:
`~/CONTAINER_NAME/collectorName/2022/12/1/15/filename.json`

### Supported data formats
| File format | Supported |
|-------------|-----------|
| JSON        | ✅     |
| XML         | ✅     |
| CSV         | ✅     |
| HTML        | ✅     |
| PDF         | ❌     |


###Collector options
This class defines which configurations you would like to use. If a mistake has been made, these options can be changed at any point in time. 
* `CollectorName` : This would be the unique name of the collector. This could prove useful for debugging and locating
* `ActiveUntil` : This defines when the collector will be instructed to stop collecting the samples. After this date the collector automatically shuts down.
* `IsActive` : This property defines whether the collector is currently active or not.
* `CanNotify` : Defines whether the client receives an E-Mail notification whenever the collector has reached any of the limits (listed below).
* `MaxSamplesHour` : The maximum number of samples that may be collected in one hour.
* `MaxSamplesDay` : The maximum number of samples that may be collected in one day.
* `MaxSamplesAlltime` : The maximum number of samples that may be collected until ActiveUntil has been reached.
* `SamplePercentage` : The percentage that defines how much samples are collected. Value range is between 0 and 100. A value of 0.05 will collect every 1 of 2000 items. Formula is: `percentage = 100 / FREQUENCY`

### appsettings.json

These settings must be included inside the appsettings.json of the project.
```json
"SampleCollector": {
    "AzureStorageBlob": {
      "ConnectionString": "YOUR_AZURE_API_KEY",
      "ContainerName": "CONTAINER_NAME",
    },
    "Database": {
      "ConnectionString": "DATABASE_CONNECTION_STRING"
    },
    "CacheUpdater": {
      "Interval": "01:00:00" // Refreshing interval of 1 hour
    }
```
The CacheUpdater:Interval property defines how often the application scans for new changes within the CollectorOptions. After this value is set it can't be re-configured while in runtime mode. 
The recommended value for this property is 1 hour which can be written as `01:00:00`.
##Setup instructions
First of all, a DB connection has to be established

* appsettings.json
  * Configure these settings as shown above


* Startup.cs :

```c#
services.Configure<AzureStorageOptions>(Configuration.GetSection("SampleCollector:AzureStorageBlob"));
services.Configure<CacheMonitorOptions>(Configuration.GetSection("SampleCollector:CacheUpdater"));
services.ConfigureSampleCollectorServices();
```

* After dependency injection of ISampleCollector within SomeClass.cs :
```c#
//this will be the data that's going to be sampled (if meets criteria)
var stringData = "{\"StudentName\" : \"John Doe\"}";
var data = new BinaryData(stringData); 

//the options that are required for creating an Collector
var options = new CollectorOptions
            {
                CollectorName = "collectorName",
                ActiveUntil = DateTime.Now.Add(TimeSpan.FromDays(15)),
                IsActive = true,
                CanNotify = false,
                MaxSamplesHour = 15,
                //MaxSamplesDay = 150,
                MaxSamplesAlltime = 2500,
                SamplePercentage = new decimal(0.05)
            };

//We simply pass the data and the options as parameters to start sampling:

 //BinaryData as parameter
_sampleCollector.ProcessMessage(binaryData, options); 
 //OR
 //string as parameter
_sampleCollector.ProcessMessage(stringData, options); 
```

After running the program the options will be synced to the database:

| CollectorName | ActiveUntil                        | IsActive | CanNotify | MaxSamplesHour | MaxSamplesDay | MaxSamplesAlltime | SamplePercentage |
|---------------|------------------------------------|----------|-----------|----------------|---------------|-------------------|------------------|
| collectorName | 2022-12-12 00:00:00.0000000 +01:00 | true     | false     | 15             | null          | 2500              | 0.05             |