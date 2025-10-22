// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: d9aa8f9a1b7d34ca9d18484a14275c1115305394d4bbed74abd97a6feec034b8
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.IoT.Deployment.Admin.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LagoVista.Core;
using System.Threading.Tasks;
using LagoVista.Core.Models;
using LagoVista.Core.Validation;

namespace LagoVista.IoT.Deployment.Tests.Validation
{
    /* Makes sure we don't have more than one listener on a port */

    [TestClass]
    public class ListenerValidationValidation : ValidationTestBase
    {

        [TestMethod]
        public void InstanceValidation_ListenerUniquePorts_Valid()
        {
            var instance = new DeploymentInstance()
            {
                Id = Guid.NewGuid().ToId(),
                CreationDate = DateTime.UtcNow.ToJSONString(),
                LastUpdatedDate = DateTime.UtcNow.ToJSONString(),
                LastUpdatedBy = EntityHeader.Create("6DA61E6EA91448618F2248C97C354F1C", "USER"),
                CreatedBy = EntityHeader.Create("6DA61E6EA91448618F2248C97C354F1C", "CREATED"),
                OwnerOrganization = EntityHeader.Create("1112403B28644ED180465C0393F0CA14", "USER"),
                Key = "instancekey",
                Name = "MyInstance",
                NuvIoTEdition = EntityHeader<NuvIoTEditions>.Create(NuvIoTEditions.App),
                ContainerRepository = new EntityHeader() { Id = "2212403B28644ED180465C0393F0CA14", Text = "Container" },
                ContainerTag = new EntityHeader() { Id = "3312403B28644ED180465C0393F0CA14", Text = "ContainerTag" },
                CloudProvider = new EntityHeader() { Id = "4412403B28644ED180465C0393F0CA14", Text = "CloudProvider" },
                Subscription = new EntityHeader() { Id = "7712403B28644ED180465C0393F0CA14", Text = "MySubscription" },
                Size = new EntityHeader() { Id = "9912403B28644ED180465C0393F0CA14", Text = "MySubscription" },
                DeviceRepository = new EntityHeader<DeviceManagement.Core.Models.DeviceRepository>()
                {
                    Id = "AA12403B28644ED180465C0393F0CA14",
                    Text = "DeviceRepo",
                    Value = new DeviceManagement.Core.Models.DeviceRepository()
                    {

                    }
                },
                Solution = new EntityHeader<Solution>()
                {
                    Id = "BB12403B28644ED180465C0393F0CA14",
                    Text ="MySolution",
                    Value = new Solution()
                    {
                        Id = "BB12403B28644ED180465C0393F0CA14",
                        CreationDate = DateTime.UtcNow.ToJSONString(),
                        LastUpdatedDate = DateTime.UtcNow.ToJSONString(),
                        LastUpdatedBy = EntityHeader.Create("6DA61E6EA91448618F2248C97C354F1C", "USER"),
                        CreatedBy = EntityHeader.Create("6DA61E6EA91448618F2248C97C354F1C", "CREATED"),
                        OwnerOrganization = EntityHeader.Create("1112403B28644ED180465C0393F0CA14", "USER"),
                        Key = "solutionkey",
                        Name = "MySolution",
                        Planner = new EntityHeader<Pipeline.Admin.Models.PlannerConfiguration>()
                        {
                            Id = "07F5FD0D4D734AC48110-184DCB20F285",
                            Text = "MyPlanner",
                            Value = new Pipeline.Admin.Models.PlannerConfiguration()
                            {
                                Id = "07F5FD0D4D734AC48110184DCB20F285",
                                Name = "MyPlanner",
                                CreationDate = DateTime.UtcNow.ToJSONString(),
                                LastUpdatedDate = DateTime.UtcNow.ToJSONString(),
                                LastUpdatedBy = EntityHeader.Create("6DA61E6EA91448618F2248C97C354F1C", "USER"),
                                CreatedBy = EntityHeader.Create("6DA61E6EA91448618F2248C97C354F1C", "CREATED"),
                                OwnerOrganization = EntityHeader.Create("1112403B28644ED180465C0393F0CA14", "USER"),
                                Key = "plannerkey",
                            }
                        }
                    }
                }
            };

            var result = Validator.Validate(instance, Actions.Any);

            //AssertIsValid(result);
        }

    }
}
