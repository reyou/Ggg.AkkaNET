using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Akka.Actor;
using Akka.Configuration;
using Akka.TestKit;
using Akka.TestKit.Xunit2;
using Akka.Util.Internal;
using FluentAssertions;
using intro.Models;
using intro.UntypedActors;

namespace intro
{
    /// <summary>
    /// https://getakka.net/articles/intro/tutorial-4.html
    /// </summary>
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    [SuppressMessage("ReSharper", "FunctionRecursiveOnAllPaths")]
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("1- DeviceGroupQuery_must_return_temperature_value_for_working_devices");
            Console.WriteLine("2- DeviceGroupQuery_must_return_TemperatureNotAvailable_for_devices_with_no_readings");
            Console.WriteLine("3- DeviceGroupQuery_must_return_return_DeviceNotAvailable_if_device_stops_before_answering");
            Console.WriteLine("4- DeviceGroupQuery_must_return_temperature_reading_even_if_device_stops_after_answering");
            Console.WriteLine("5- DeviceGroup_actor_must_be_able_to_collect_temperatures_from_all_active_devices");
            string readLine = Console.ReadLine();
            switch (readLine)
            {
                case "1":
                    DeviceGroupQuery_must_return_temperature_value_for_working_devices();
                    break;
                case "2":
                    DeviceGroupQuery_must_return_TemperatureNotAvailable_for_devices_with_no_readings();
                    break;
                case "3":
                    DeviceGroupQuery_must_return_return_DeviceNotAvailable_if_device_stops_before_answering();
                    break;
                case "4":
                    DeviceGroupQuery_must_return_temperature_reading_even_if_device_stops_after_answering();
                    break;
                case "5":
                    DeviceGroup_actor_must_be_able_to_collect_temperatures_from_all_active_devices();
                    break;

            }
            Console.WriteLine("");
            Console.WriteLine("App is completed.");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Main(null);
        }

        private static void DeviceGroup_actor_must_be_able_to_collect_temperatures_from_all_active_devices()
        {
            TestProbe probe = CreateTestProbe();
            TestKit testKit = new TestKit();
            IActorRef groupActor = testKit.Sys.ActorOf(DeviceGroup.Props("group"));
            groupActor.Tell(new RequestTrackDevice("group", "device1"), probe.Ref);
            probe.ExpectMsg<DeviceRegistered>();
            IActorRef deviceActor1 = probe.LastSender;

            groupActor.Tell(new RequestTrackDevice("group", "device2"), probe.Ref);
            probe.ExpectMsg<DeviceRegistered>();
            IActorRef deviceActor2 = probe.LastSender;

            groupActor.Tell(new RequestTrackDevice("group", "device3"), probe.Ref);
            probe.ExpectMsg<DeviceRegistered>();
            IActorRef deviceActor3 = probe.LastSender;

            // Check that the device actors are working
            deviceActor1.Tell(new RecordTemperature(requestId: 0, value: 1.0), probe.Ref);
            probe.ExpectMsg<TemperatureRecorded>(s => s.RequestId == 0);
            deviceActor2.Tell(new RecordTemperature(requestId: 1, value: 2.0), probe.Ref);
            probe.ExpectMsg<TemperatureRecorded>(s => s.RequestId == 1);

            groupActor.Tell(new RequestAllTemperatures(0), probe.Ref);
            probe.ExpectMsg<RespondAllTemperatures>(msg =>
                msg.Temperatures["device1"].AsInstanceOf<Temperature>().Value == 1.0 &&
                msg.Temperatures["device2"].AsInstanceOf<Temperature>().Value == 2.0 &&
                msg.Temperatures["device3"] is TemperatureNotAvailable &&
                msg.RequestId == 0);


        }

        private static void DeviceGroupQuery_must_return_temperature_reading_even_if_device_stops_after_answering()
        {
            TestProbe requester = CreateTestProbe();
            TestProbe device1 = CreateTestProbe();
            TestProbe device2 = CreateTestProbe();
            TestKit testKit = new TestKit();
            Dictionary<IActorRef, string> actorToDeviceId = new Dictionary<IActorRef, string>
            {
                [device1.Ref] = "device1",
                [device2.Ref] = "device2"
            };
            TimeSpan timeout = TimeSpan.FromSeconds(3);
            long requestId = 1;
            IActorRef requesterParam = requester.Ref;
            IActorRef queryActor = testKit.Sys.ActorOf(DeviceGroupQuery.Props(
                actorToDeviceId,
                requestId,
                requesterParam,
                timeout
            ));
            device1.ExpectMsg<ReadTemperature>(read => read.RequestId == 0);
            device2.ExpectMsg<ReadTemperature>(read => read.RequestId == 0);

            queryActor.Tell(new RespondTemperature(requestId: 0, value: 1.0), device1.Ref);
            queryActor.Tell(new RespondTemperature(requestId: 0, value: 2.0), device2.Ref);
            device2.Tell(PoisonPill.Instance);

            requester.ExpectMsg<RespondAllTemperatures>(msg =>
                msg.Temperatures["device1"].AsInstanceOf<Temperature>().Value == 1.0 &&
                msg.Temperatures["device2"].AsInstanceOf<Temperature>().Value == 2.0 &&
                msg.RequestId == 1);
        }

        private static void DeviceGroupQuery_must_return_return_DeviceNotAvailable_if_device_stops_before_answering()
        {
            TestProbe requester = CreateTestProbe();
            TestProbe device1 = CreateTestProbe();
            TestProbe device2 = CreateTestProbe();
            TestKit testKit = new TestKit();
            Dictionary<IActorRef, string> actorToDeviceId = new Dictionary<IActorRef, string>
            {
                [device1.Ref] = "device1",
                [device2.Ref] = "device2"
            };
            TimeSpan timeout = TimeSpan.FromSeconds(3);
            long requestId = 1;
            IActorRef requesterParam = requester.Ref;
            IActorRef queryActor = testKit.Sys.ActorOf(DeviceGroupQuery.Props(
                actorToDeviceId,
                requestId,
                requesterParam,
                timeout: timeout
            ));
            device1.ExpectMsg<ReadTemperature>(read => read.RequestId == 0);
            device2.ExpectMsg<ReadTemperature>(read => read.RequestId == 0);

            queryActor.Tell(new RespondTemperature(requestId: 0, value: 1.0), device1.Ref);
            device2.Tell(PoisonPill.Instance);

            requester.ExpectMsg<RespondAllTemperatures>(msg =>
                msg.Temperatures["device1"].AsInstanceOf<Temperature>().Value == 1.0 &&
                msg.Temperatures["device2"] is DeviceNotAvailable &&
                msg.RequestId == 1);
        }

        private static void DeviceGroupQuery_must_return_TemperatureNotAvailable_for_devices_with_no_readings()
        {
            TestProbe requester = CreateTestProbe();

            TestProbe device1 = CreateTestProbe();
            TestProbe device2 = CreateTestProbe();
            TestKit testKit = new TestKit();
            Dictionary<IActorRef, string> actorToDeviceId = new Dictionary<IActorRef, string>
            {
                [device1.Ref] = "device1",
                [device2.Ref] = "device2"
            };
            TimeSpan timeout = TimeSpan.FromSeconds(3);
            long requestId = 1;
            IActorRef requesterParam = requester.Ref;
            IActorRef queryActor = testKit.Sys.ActorOf(DeviceGroupQuery.Props(
                actorToDeviceId,
                requestId,
                requesterParam,
                timeout
            ));
            device1.ExpectMsg<ReadTemperature>(read => read.RequestId == 0);
            device2.ExpectMsg<ReadTemperature>(read => read.RequestId == 0);

            queryActor.Tell(new RespondTemperature(requestId: 0, value: null), device1.Ref);
            queryActor.Tell(new RespondTemperature(requestId: 0, value: 2.0), device2.Ref);

            requester.ExpectMsg<RespondAllTemperatures>(msg =>
                msg.Temperatures["device1"] is TemperatureNotAvailable &&
                msg.Temperatures["device2"].AsInstanceOf<Temperature>().Value == 2.0 &&
                msg.RequestId == 1);
        }

        private static void DeviceGroupQuery_must_return_temperature_value_for_working_devices()
        {
            TestProbe requester = CreateTestProbe();
            TestProbe device1 = CreateTestProbe();
            TestProbe device2 = CreateTestProbe();
            TestKit testKit = new TestKit();
            Dictionary<IActorRef, string> actorToDeviceId = new Dictionary<IActorRef, string>
            {
                [device1.Ref] = "device1",
                [device2.Ref] = "device2"
            };
            TimeSpan timeout = TimeSpan.FromSeconds(3);
            Props props = DeviceGroupQuery.Props(actorToDeviceId, 1, requester.Ref, timeout);
            IActorRef queryActor = testKit.Sys.ActorOf(props);
            device1.ExpectMsg<ReadTemperature>(read => read.RequestId == 0);
            device2.ExpectMsg<ReadTemperature>(read => read.RequestId == 0);
            queryActor.Tell(new RespondTemperature(requestId: 0, value: 1.0), device1.Ref);
            queryActor.Tell(new RespondTemperature(requestId: 0, value: 2.0), device2.Ref);
            requester.ExpectMsg<RespondAllTemperatures>(msg =>
                msg.Temperatures["device1"].AsInstanceOf<Temperature>().Value == 1.0 &&
                msg.Temperatures["device2"].AsInstanceOf<Temperature>().Value == 2.0 &&
                msg.RequestId == 1);
        }

        private static TestProbe CreateTestProbe()
        {
            TestKit testKit = new TestKit();
            TestProbe probe = testKit.CreateTestProbe("test-probe");
            return probe;
        }

        static void Main2(string[] args)
        {
            TestKit testKit = new TestKit();
            ActorSystem actorSystem = testKit.Sys;
            using (actorSystem)
            {
                TestProbe probe = testKit.CreateTestProbe("test-probe");

                /*In the first, we just test that we get back the list of
                 proper IDs once we have added a few devices.*/
                Action test1 = new Action(() =>
                {
                    IActorRef deviceManagerActor = actorSystem.ActorOf(DeviceManager.Props("device-manager"));

                    deviceManagerActor.Tell(new RequestTrackDevice("group", "device1"), probe.Ref);
                    probe.ExpectMsg<DeviceRegistered>();

                    deviceManagerActor.Tell(new RequestTrackDevice("group", "device2"), probe.Ref);
                    probe.ExpectMsg<DeviceRegistered>();

                    deviceManagerActor.Tell(new RequestDeviceList(requestId: 0), probe.Ref);
                    probe.ExpectMsg<ReplyDeviceList>(s => s.RequestId == 0
                                                          && s.Ids.Contains("device1")
                                                          && s.Ids.Contains("device2"));
                    Console.WriteLine("Test 1 passed.");
                    Console.WriteLine("");
                });
                test1.Invoke();

                /*The second test case makes sure that the device ID
                 is properly removed after the device actor has been stopped.*/
                Action test2 = new Action(() =>
                {
                    IActorRef deviceManagerActor = actorSystem.ActorOf(DeviceManager.Props("device-manager"));

                    deviceManagerActor.Tell(new RequestTrackDevice("group", "device1"), probe.Ref);
                    probe.ExpectMsg<DeviceRegistered>();
                    IActorRef toShutDown = probe.LastSender;

                    deviceManagerActor.Tell(new RequestTrackDevice("group", "device2"), probe.Ref);
                    probe.ExpectMsg<DeviceRegistered>();

                    deviceManagerActor.Tell(new RequestDeviceList(requestId: 0), probe.Ref);
                    probe.ExpectMsg<ReplyDeviceList>(s => s.RequestId == 0
                                                          && s.Ids.Contains("device1")
                                                          && s.Ids.Contains("device2"));

                    probe.Watch(toShutDown);
                    toShutDown.Tell(PoisonPill.Instance);
                    probe.ExpectTerminated(toShutDown);

                    // using awaitAssert to retry because it might take longer for the groupActor
                    // to see the Terminated, that order is undefined
                    probe.AwaitAssert(() =>
                    {
                        deviceManagerActor.Tell(new RequestDeviceList(requestId: 1), probe.Ref);
                        probe.ExpectMsg<ReplyDeviceList>(s => s.RequestId == 1 && s.Ids.Contains("device2"));
                    });

                    Console.WriteLine("Test 2 passed.");
                    Console.WriteLine("");
                });
                test2.Invoke();
                Console.WriteLine("UserMessage: App is finished.");
                // Exit the system after ENTER is pressed
                Console.ReadLine();
            }

        }
    }


}
