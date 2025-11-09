// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 4ac267ee2d3d9b7e28f9a9c6b2ecdc1f2effb2c10baecb940aac259fa82d2665
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Validation;
using LagoVista.IoT.Deployment.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.IoT.Deployment.Tests.DeviceConfig
{
    [TestClass]
    public class WatchDogExclusionValidationTests
    {
        [TestMethod]
        public void Should_Be_Valid_Exclusion()
        {
            var wde = new WatchdogExclusion();
            wde.Start = 0;
            wde.End = 730;
            var result = new ValidationResult();
            wde.Validate(result);
            Assert.IsTrue(result.Successful);
        }

        [TestMethod]
        public void Should_Not_Be_Valid_Exclusion_When_Start_Is_Greater_Then_End()
        {
            var wde = new WatchdogExclusion();
            wde.Start = 730;
            wde.End = 0;
            var result = new ValidationResult();
            wde.Validate(result);
            Assert.IsFalse(result.Successful);
            Console.WriteLine(result.Errors.First().ToString());
        }

        [TestMethod]
        public void Should_Not_Be_Valid_Exclusion_When_Start_Is_Equal_To_End()
        {
            var wde = new WatchdogExclusion();
            wde.Start = 730;
            wde.End = 730;
            var result = new ValidationResult();
            wde.Validate(result);
            Assert.IsFalse(result.Successful);
            Console.WriteLine(result.Errors.First().ToString());
        }

        [TestMethod]
        public void Should_Not_Be_Valid_Exclusion_When_Start_Is_Invalid_Minute()
        {
            var wde = new WatchdogExclusion();
            wde.Start = 78;
            wde.End = 730;
            var result = new ValidationResult();
            wde.Validate(result);
            Assert.IsFalse(result.Successful);
            Console.WriteLine(result.Errors.First().ToString());
        }

        [TestMethod]
        public void Should_Not_Be_Valid_Exclusion_When_End_Is_Invalid_Minute()
        {
            var wde = new WatchdogExclusion();
            wde.Start = 0;
            wde.End = 778;
            var result = new ValidationResult();
            wde.Validate(result);
            Assert.IsFalse(result.Successful);
            Console.WriteLine(result.Errors.First().ToString());
        }

        [TestMethod]
        public void Should_Not_Be_Valid_Exclusion_When_Start_Is_Invalid_Hours()
        {
            var wde = new WatchdogExclusion();
            wde.Start = 3278;
            wde.End = 730;
            var result = new ValidationResult();
            wde.Validate(result);
            Assert.IsFalse(result.Successful);
            Console.WriteLine(result.Errors.First().ToString());
        }

        [TestMethod]
        public void Should_Not_Be_Valid_Exclusion_When_End_Is_Invalid_Hours()
        {
            var wde = new WatchdogExclusion();
            wde.Start = 0;
            wde.End = 3230;
            var result = new ValidationResult();
            wde.Validate(result);
            Assert.IsFalse(result.Successful);
            Console.WriteLine(result.Errors.First().ToString());
        }

    }
}
