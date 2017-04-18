using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Azure.Devices.Applications.RemoteMonitoring.Common.Exceptions;
using Microsoft.Azure.Devices.Applications.RemoteMonitoring.Common.Extensions;
using Microsoft.Azure.Devices.Applications.RemoteMonitoring.Common.Helpers;
using Microsoft.Azure.Devices.Applications.RemoteMonitoring.Common.Models;
using Microsoft.Azure.Devices.Applications.RemoteMonitoring.Common.Models.Commands;
using Microsoft.Azure.Devices.Shared;

namespace Microsoft.Azure.Devices.Applications.RemoteMonitoring.Common.Factory
{
    public static class SmartCoolerFactory
    {
        public const string OBJECT_TYPE_DEVICE_INFO = "DeviceInfo";

        public const string VERSION_1_0 = "1.0";

        private const int MAX_COMMANDS_SUPPORTED = 6;

        private const bool IS_SIMULATED_DEVICE = true;

        private static readonly Random Rand = new Random();

        private static readonly List<string> DefaultDeviceNames = new List<string>
        {
            "SmartCooler001",
            "SmartCooler002",
            "SmartCooler003",
            "SmartCooler004",
            "SmartCooler005",
            "SmartCooler006",
            "SmartCooler007",
            "SmartCooler008",
            "SmartCooler009",
            "SmartCooler010",
            "SmartCooler011",
            "SmartCooler012",
            "SmartCooler013",
            "SmartCooler014",
            "SmartCooler015",
            "SmartCooler016",
            "SmartCooler017",
            "SmartCooler018",
            "SmartCooler019",
            "SmartCooler020",
            "SmartCooler021",
            "SmartCooler022",
            "SmartCooler023",
            "SmartCooler024",
            "SmartCooler025"
        };

        private static readonly List<string> FreeFirmwareDeviceNames = new List<string>
        {
            "SmartCooler001",
            "SmartCooler002",
            "SmartCooler003",
            "SmartCooler004",
            "SmartCooler005",
            "SmartCooler006",
            "SmartCooler007",
            "SmartCooler008"
        };

        private static readonly List<string> HighTemperatureDeviceNames = new List<string>
        {
            "SmartCooler001",
            "SmartCooler002",
            "SmartCooler003",
            "SmartCooler004",
            "SmartCooler005"
        };

        private class Location
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }

            public Location(double latitude, double longitude)
            {
                Latitude = latitude;
                Longitude = longitude;
            }

        }

        private static List<Location> _possibleDeviceLocations = new List<Location>{
            new Location(47.659159, -122.141515),  // Microsoft Red West Campus, Representative A
            new Location(47.593307, -122.332165),  // 800 Occidental Ave S, Seattle, WA 98134
            new Location(47.617025, -122.191285),  // 11111 NE 8th St, Bellevue, WA 98004
            new Location(47.583582, -122.130622),  // 3003 160th Ave SE Bellevue, WA 98008
            new Location(47.639511, -122.134376),  // 15580 NE 31st St Redmond, WA 98008
            new Location(47.644328, -122.137036),  // 15255 NE 40th St Redmond, WA 98008
            new Location(47.621573, -122.338101),  // 320 Westlake Ave N, Seattle, WA 98109
            new Location(47.642357, -122.137152), // 15010 NE 36th St, Redmond, WA 98052
            new Location(47.614981, -122.195781), //500 108th Ave NE, Bellevue, WA 98004
            new Location(47.642528, -122.130565), //3460 157th Ave NE, Redmond, WA 98052
            new Location(47.617187, -122.191685), //11155 NE 8th St, Bellevue, WA 98004
            new Location(47.677292, -122.093030), //18500 NE Union Hill Rd, Redmond, WA 98052
            new Location(47.642528, -122.130565), //3600 157th Ave NE, Redmond, WA 98052
            new Location(47.642876, -122.125492), //16070 NE 36th Way Bldg 33, Redmond, WA 98052
            new Location(47.637376, -122.140445), //14999 NE 31st Way, Redmond, WA 98052
            new Location(47.636121, -122.130254) //3009 157th Pl NE, Redmond, WA 98052
        };

        private static List<string> _possibleRepresentativeTags = new List<string>
        {
            "Representative 40",
            "Representative 43"
        };

        private static List<string> _possibleFloorTags = new List<string>
        {
            "1F",
            "2F",
        };

        public static DeviceModel GetSampleSimulatedDevice(string deviceId, string key)
        {
            DeviceModel device = DeviceCreatorHelper.BuildDeviceStructure(deviceId, true, null);

            AssignDeviceProperties(device);
            device.ObjectType = OBJECT_TYPE_DEVICE_INFO;
            device.Version = VERSION_1_0;
            device.IsSimulatedDevice = IS_SIMULATED_DEVICE;

            AssignTelemetry(device);
            AssignCommands(device);

            return device;
        }

        public static DeviceModel GetSampleDevice(Random randomNumber, SecurityKeys keys)
        {
            var deviceId = string.Format(
                    CultureInfo.InvariantCulture,
                    "00000-DEV-{0}C-{1}LK-{2}D-{3}",
                    MAX_COMMANDS_SUPPORTED,
                    randomNumber.Next(99999),
                    randomNumber.Next(99999),
                    randomNumber.Next(99999));

            var device = DeviceCreatorHelper.BuildDeviceStructure(deviceId, false, null);
            device.ObjectName = "IoT Device Description";

            AssignDeviceProperties(device);
            AssignTelemetry(device);
            AssignCommands(device);

            return device;
        }

        private static void AssignDeviceProperties(DeviceModel device)
        {
            int randomId = Rand.Next(0, _possibleDeviceLocations.Count - 1);
            if (device?.DeviceProperties == null)
            {
                throw new DeviceRequiredPropertyNotFoundException("Required DeviceProperties not found");
            }

            device.DeviceProperties.HubEnabledState = true;
            device.DeviceProperties.Manufacturer = "Contoso Inc.";
            device.DeviceProperties.ModelNumber = "MD-" + randomId;
            device.DeviceProperties.SerialNumber = "SER" + randomId;

            if (FreeFirmwareDeviceNames.Any(n => device.DeviceProperties.DeviceID.StartsWith(n, StringComparison.Ordinal)))
            {
                device.DeviceProperties.FirmwareVersion = "1." + randomId;
            }
            else
            {
                device.DeviceProperties.FirmwareVersion = "2.0";
            }

            device.DeviceProperties.Platform = "Plat-" + randomId;
            device.DeviceProperties.Processor = "i3-" + randomId;
            device.DeviceProperties.InstalledRAM = randomId + " MB";

            // Choose a location among the 16 above and set Lat and Long for device properties
            device.DeviceProperties.Latitude = _possibleDeviceLocations[randomId].Latitude;
            device.DeviceProperties.Longitude = _possibleDeviceLocations[randomId].Longitude;
        }

        private static void AssignTelemetry(DeviceModel device)
        {
            device.Telemetry.Add(new Telemetry("Temperature", "Temperature", "double"));
            device.Telemetry.Add(new Telemetry("Humidity", "Humidity", "double"));
            device.Telemetry.Add(new Telemetry("Weight", "Weight", "double"));
        }

        private static void AssignCommands(DeviceModel device)
        {
            // Device commands
            device.Commands.Add(new Command(
                "PingDevice",
                DeliveryType.Message,
                "The device responds to this command with an acknowledgement. This is useful for checking that the device is still active and listening."
            ));
            device.Commands.Add(new Command(
                "StartTelemetry",
                DeliveryType.Message,
                "Instructs the device to start sending telemetry."
            ));
            device.Commands.Add(new Command(
                "StopTelemetry",
                DeliveryType.Message,
                "Instructs the device to stop sending telemetry."
            ));
            device.Commands.Add(new Command(
                "ChangeSetPointTemp",
                DeliveryType.Message,
                "Controls the simulated temperature telemetry values the device sends. This is useful for testing back-end logic.",
                new[] { new Parameter("SetPointTemp", "double") }
            ));
            device.Commands.Add(new Command(
                "DiagnosticTelemetry",
                DeliveryType.Message,
                "Controls if the device should send the external temperature as telemetry.",
                new[] { new Parameter("Active", "boolean") }
            ));
            device.Commands.Add(new Command(
                "ChangeDeviceState",
                DeliveryType.Message,
                "Sets the device state metadata property that the device reports. This is useful for testing back-end logic.",
                new[] { new Parameter("DeviceState", "string") }
            ));

            // Device methods
            device.Commands.Add(new Command(
                "InitiateFirmwareUpdate",
                DeliveryType.Method,
                "Updates device Firmware. Use parameter 'FwPackageUri' to specifiy the URI of the firmware file, e.g. https://iotrmassets.blob.core.windows.net/firmwares/FW20.bin",
                new[] { new Parameter("FwPackageUri", "string") }
            ));
            device.Commands.Add(new Command(
                "Reboot",
                DeliveryType.Method,
                "Reboot the device"
            ));
            device.Commands.Add(new Command(
                "FactoryReset",
                DeliveryType.Method,
                "Reset the device (including firmware and configuration) to factory default state"
            ));
        }

        public static List<string> GetDefaultDeviceNames()
        {
            long milliTime = DateTime.Now.Millisecond;
            return DefaultDeviceNames.Select(r => string.Concat(r, "_" + milliTime)).ToList();
        }

        public static void AssignDefaultTags(DeviceModel device)
        {
            if (device.Twin == null)
            {
                device.Twin = new Twin();
            }

            device.Twin.Tags["Representative"] = Random(_possibleRepresentativeTags);
            device.Twin.Tags["Floor"] = Random(_possibleFloorTags);
        }

        public static void AssignDefaultDesiredProperties(DeviceModel device)
        {
            if (HighTemperatureDeviceNames.Any(n => device.DeviceProperties.DeviceID.StartsWith(n, StringComparison.Ordinal)))
            {
                if (device.Twin == null)
                {
                    device.Twin = new Twin();
                }

                device.Twin.Properties.Desired.Set("Config.TemperatureMeanValue", 10);
            }
        }

        private static T Random<T>(IList<T> range)
        {
            return range[Rand.Next(range.Count)];
        }
    }
}
