using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.IoT.Deployment.Admin.Models
{
    public class BillingEvent : TableStorageEntity
    {
        public string SubscriptionId { get; set; }


        /* OrgID is the Partition Key */
        public string OrgId { get; set; }

        /// <summary>
        /// When the billing event started
        /// </summary>
        public string StartTimeStamp { get; set; }

        /// <summary>
        /// When the billing event ended
        /// </summary>
        public string EndTimeStamp { get; set; }

        /// <summary>
        /// When the EndTimeStamp is assigned we will calculte the number of hours the resource has been
        /// used, this will be used to calculate the price/cost
        /// </summary>
        public double? HoursBilled { get; set; }

        /// <summary>
        /// Name of the SKU that makes up the product
        /// </summary>
        public string SkuId { get; set; }

        /// <summary>
        /// Name of offerred product
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Cost Per Unit
        /// </summary>
        public double? CostPerHour { get; set; }

        /// <summary>
        /// Applied Discounts
        /// </summary>
        public double? Discount { get; set; }

        /// <summary>
        /// Extended price for this billing period
        /// </summary>
        public double? Extended { get; set; }

        /// <summary>
        /// Actual resource that was used
        /// </summary>
        public string ResourceId { get; set; }

        /// <summary>
        /// Optional user entered notes
        /// </summary>
        public string Notes { get; set; }
    }
}
