using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using GrpcRepGenServer;
using System;
using System.Threading.Tasks;


namespace grpTestClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //OPEN A CHANNEL LT. UHRA
            GrpcChannel channel = GrpcChannel.ForAddress("https://localhost:5001");
            
            //MAKE A CLIENTS FOR THE SERVICES - IN THIS CASE THERE ARE TWO SERVICES ON THE SERVER
            GrpcRepGenServer.Greeter.GreeterClient client_Greeter = new GrpcRepGenServer.Greeter.GreeterClient(channel);
            GrpcRepGenServer.HealthChecker.HealthCheckerClient client_HeatlhCheck = new GrpcRepGenServer.HealthChecker.HealthCheckerClient(channel);
            GrpcRepGenServer.ReportMaker.ReportMakerClient client_Reporter = new GrpcRepGenServer.ReportMaker.ReportMakerClient(channel);
           
            //MAKE SOME MESSAGES
            var request_Hello = new GrpcRepGenServer.HelloRequest{ Name = "Central"};
            var request_Health = new GrpcRepGenServer.HealthRequest { Name = "HealthAdmin" };
            var request_ToolHistory = new GrpcRepGenServer.ToolHistoryRequest { ToolID = "ZT09",
                                                                                TimeStart = Timestamp.FromDateTime(DateTime.SpecifyKind(new DateTime(2000, 2, 28, 2, 2, 2), DateTimeKind.Utc)),
                                                                                TimeEnd = Timestamp.FromDateTime(DateTime.UtcNow)
                                                                               };

            //SEND THE REQUEST MESSAGES AND LOOK FOR RESPONSES
            var response_Greet = await client_Greeter.SayHelloAsync(request_Hello);
            var response_Health = await client_HeatlhCheck.ReportHealthAsync(request_Health);
            var response_ToolHist = await client_Reporter.ReportToolProcessHistoryAsync(request_ToolHistory);


            //PRINT TO CONSOLE
            Console.WriteLine($"Response from Greeter Service: {response_Greet.Message}");
            Console.WriteLine($"Response from Health Service: {response_Health.Message}");
            Console.WriteLine("Health Data " + "[Message]:" + response_Health.Message + 
                                            " [TimeStamp]:" + response_Health.Timestamp + 
                                            " [ErrorCount]:" + response_Health.NumErrors.ToString());
            
            Console.WriteLine("Tool History Data " + "[Message]:" + response_ToolHist.Message +
                                            " [TimeStart]:" + response_ToolHist.TimeStart.ToString() +
                                            " [TimeEnd]:" + response_ToolHist.TimeEnd.ToString());

            foreach(toolEvents tevent in response_ToolHist.ToolEvents)
            {
                Console.WriteLine("[Tool Event Time]:" + tevent.EventTime.ToString() + " [Event Type]:" + tevent.EventType.ToString());

            }

           



            //WAIT FOR AN "ENTER" SO WE CAN READ ABOUT IT
            Console.ReadLine();

        }//MAIN

    }//CLASS
}//NAMESPACE
