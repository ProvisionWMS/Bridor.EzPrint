using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Bridor.EzPrint.Resources;

namespace Bridor.EzPrint.Model
{
    public class LabelData : INotifyPropertyChanged, IDataErrorInfo
    {
        private int quantity;
        public int Quantity {
            get { return this.quantity; }
            set {
                if (this.quantity != value) {
                    this.quantity = value;
                    OnPropertyChange("Quantity");
                }
            }
        }
        private int backday;
        public int Backday {
            get { return this.backday; }
            set {
                if (this.backday != value) {
                    this.backday = value;
                    OnPropertyChange("Backday");
                }
            }
        }
        private int? workOrderNumber;
        public int? WorkOrderNumber
        {
            get { return this.workOrderNumber; }
            set
            {
                if (this.workOrderNumber != value)
                {
                    this.workOrderNumber = value;
                    OnPropertyChange("WorkOrderNumber");
                }
            }
        }

        private string workOrderNumberStr;
        public string WorkOrderNumberStr
        {
            get { return this.workOrderNumberStr; }
            set
            {
                if (this.workOrderNumberStr != value)
                {
                    this.workOrderNumberStr = value;
                    OnPropertyChange("WorkOrderNumberStr");
                }
            }
        }

        private string productNumber;
        public string ProductNumber {
            get { return this.productNumber; }
            set {
                if (String.IsNullOrEmpty(this.productNumber) || this.productNumber.CompareTo(value) != 0) {
                    this.productNumber = value;
                    OnPropertyChange("ProductNumber");
                }
            }
        }
        private string productionLine;
        public string ProductionLine {
            get { return this.productionLine; }
            set {
                if (String.IsNullOrEmpty(this.productionLine) || this.productionLine.CompareTo(value) != 0) {
                    this.productionLine = value;
                    OnPropertyChange("ProductionLine");
                }
            }
        }
        private string description;
        public string Description {
            get { return this.description; }
            set {
                if (String.IsNullOrEmpty(this.description) || this.description.CompareTo(value) != 0) {
                    this.description = value;
                    OnPropertyChange("Description");
                }
            }
        }
        private DateTime? expiryDate;
        public DateTime? ExipryDate {
            get { return this.expiryDate; }
            set {
                if (!this.expiryDate.HasValue || this.expiryDate.Value.CompareTo(value) != 0) {
                    this.expiryDate = value;
                    OnPropertyChange("ExipryDate");
                }
            }
        }
        public Template Template { get; set; }

        #region -  INotifyPropertyChanged Members  -
        /// <summary>
        /// Event raised when a property is changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Method to raise the event when a property changes
        /// </summary>
        /// <param name="propertyName">Name of the proeprty tha changed</param>
        private void OnPropertyChange(string propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region -  IDataErrorInfo Members  -
        /// <summary>
        /// Get a value indicating if there are errors int he class
        /// </summary>
        public string Error {
            get { return (this.errors.Count > 0) ? "Y" : String.Empty ; }
        }
        private IList<string> errors = new List<string>();
        /// <summary>
        /// Get the error message for the selected property
        /// </summary>
        /// <param name="columnName">Property that has the error</param>
        /// <returns></returns>
        public string this[string columnName] {
            get {
                // Do not validate if the template is null
                if (this.Template == null) return String.Empty;

                //int iWoNumber = 0;
                long iWoNumber = 0; //change 2023-08-28 by JB

                // Value to return in case of error
                string result = String.Empty;
                switch (columnName) {
                    case "Quantity":
                        if (this.Quantity < 1 || this.Quantity > Properties.Settings.Default.PrintMaxQty) {
                            // The quantity is invalid
                            result = String.Format(UIResources.ValidationErrorQuantity,
                                Properties.Settings.Default.PrintMaxQty);
                        }
                        break;
                    case "Backday":
                        if (this.Backday < -360 || this.Backday > 360) {
                            // Backday value is invalid
                            result = UIResources.ValidationErrorBackday;
                        }
                        break;
                    case "WorkOrderNumberStr":
                        
                        //2021-05-27
                        //if (this.Template.IsWorkOrderNumberActive && !this.WorkOrderNumber.HasValue)
                        //{
                        //    result = UIResources.ValidationErrorWorkOrder;
                        //    break;
                        //}

                        //if (this.Template.IsWorkOrderNumberActive && (!this.WorkOrderNumber.HasValue || (this.WorkOrderNumber < 100000 || this.WorkOrderNumber > 200000))) {
                        //    // WorkOrder number value is invalid
                        //    if (this.WorkOrderNumber < 1000000 || this.WorkOrderNumber > 3000000)
                        //    {
                        //        if (this.workOrderNumber != 999999)
                        //        {
                        //            result = UIResources.ValidationErrorWorkOrder;
                        //        }
                        //    }
                        //}

                        //this.WorkOrderNumberStr = this.WorkOrderNumber.ToString();

                        

                        if (this.Template.IsWorkOrderNumberActive && this.WorkOrderNumberStr.Length == 0)
                        {
                            result = UIResources.ValidationErrorWorkOrder;
                            break;
                        }
                        //BF REQUIRED
                        //if (Properties.Settings.Default.BF_REQUIRED == true)
                        //{
                        if (Properties.Settings.Default.BF_BARCODE == false)
                        {
                            if (this.WorkOrderNumberStr.Length > 0)
                            {
                                try
                                {
                                    iWoNumber = Convert.ToInt64(this.WorkOrderNumberStr);
                                }
                                catch
                                {
                                    this.WorkOrderNumberStr = "";
                                    result = UIResources.ValidationErrorWorkOrder;
                                    break;
                                }
                            }

                            bool is45code = false;

                            if (this.WorkOrderNumberStr.Length > 1)
                            {
                                string prefix;
                                prefix = this.WorkOrderNumberStr.Substring(0, 2);
                                if (prefix.Contains("45") == true)
                                {
                                    is45code = true;
                                    if (this.WorkOrderNumberStr.Length != 10)
                                    {
                                        result = UIResources.ValidationErrorWorkOrder;
                                    }
                                }
                            }

                            if (is45code == true) break;

                            if (this.Template.IsWorkOrderNumberActive && (this.WorkOrderNumberStr.Length == 0 || (iWoNumber < 100000 || iWoNumber > 200000)))
                            {
                                // WorkOrder number value is invalid
                                if (iWoNumber < 1000000 || iWoNumber > 3000000)
                                {
                                    //if (iWoNumber > 4500000000 || iWoNumber < 4600000000) //add 2023-08-28 by JB
                                    //{
                                    //    result = "";
                                    //}
                                    if (iWoNumber != 999999)
                                    {
                                        result = UIResources.ValidationErrorWorkOrder;
                                    }
                                }
                            }
                        }
                        else
                        {
                            string suffix;
                            string data;
                            int index = 0;
                            string value;

                            suffix = Properties.Settings.Default.BF_SUFFIX;

                            data = this.WorkOrderNumberStr;
                            index = data.IndexOf(suffix, 0);
                            if (index > 0)
                            {
                                //if (data.Contains(suffix) == true)
                                //{
                                this.Template.IsBarcodeOK = true;
                                value = data.Substring(0, index);

                                try
                                {
                                    iWoNumber = Convert.ToInt32(value);
                                }
                                catch
                                {
                                    this.WorkOrderNumberStr = "";
                                    this.Template.IsBarcodeOK = false;
                                    result = UIResources.ValidationErrorWorkOrder;
                                    break;
                                }

                                if (this.Template.IsWorkOrderNumberActive && (this.WorkOrderNumberStr.Length == 0 || (iWoNumber < 100000 || iWoNumber > 200000)))
                                {
                                    // WorkOrder number value is invalid
                                    if (iWoNumber < 1000000 || iWoNumber > 3000000)
                                    {
                                        if (iWoNumber != 999999)
                                        {
                                            this.WorkOrderNumberStr = "";
                                            this.Template.IsBarcodeOK = false;
                                            result = UIResources.ValidationErrorWorkOrder;
                                            break;
                                        }
                                    }
                                }
                                this.WorkOrderNumberStr = iWoNumber.ToString();
                                //}                         
                            }
                            //else
                            //{
                            //    this.WorkOrderNumberStr = data;
                            //}
                        }
                        //}
                        //Commented 2021-10-07 by Jerry
                        //else //BF NOT REQUIRED
                        //{
                        //    try
                        //    {
                        //        iWoNumber = Convert.ToInt32(this.WorkOrderNumberStr);
                        //    }
                        //    catch
                        //    {
                        //        this.WorkOrderNumberStr = "";
                        //        result = UIResources.ValidationErrorWorkOrder;
                        //        break;
                        //    }


                        //    if (this.Template.IsWorkOrderNumberActive && (this.WorkOrderNumberStr.Length == 0 || (iWoNumber < 100000 || iWoNumber > 200000)))
                        //    {
                        //        // WorkOrder number value is invalid
                        //        if (iWoNumber < 1000000 || iWoNumber > 3000000)
                        //        {
                        //            if (iWoNumber != 999999)
                        //            {
                        //                result = UIResources.ValidationErrorWorkOrder;
                        //            }
                        //        }
                        //    }
                        //}

                        
                        break;
                    case "ProductNumber":
                        if (this.Template.IsNewProductActive && String.IsNullOrEmpty(this.ProductNumber)) {
                            // Product code is required
                            result = UIResources.ValidationErrorProductNumber;
                        }
                        break;
                    case "ProductionLine":
                        if (this.Template.IsOriginalProductionLineActive && String.IsNullOrEmpty(this.ProductionLine)) {
                            // Production line is required
                            result = UIResources.ValidationErrorProductionLine;
                        }
                        break;
                    case "Description":
                        if (this.Template.IsNewDescriptionActive && String.IsNullOrEmpty(this.Description)) {
                            // Description is required
                            result = UIResources.ValidationErrorDescription;
                        }
                        break;
                    case "ExipryDate":
                        if (this.Template.IsExpiryDateActive && !this.ExipryDate.HasValue){
                            result = UIResources.ValidationErrorDateRequired;
                        } else if (this.Template.IsExpiryDateActive && this.ExipryDate < DateTime.Now) {
                            result = UIResources.ValidationErrorDateInvalid;
                        }
                        break;
                }

                if(String.IsNullOrEmpty(result)){
                    this.errors.Remove(columnName);
                } else {
                    if(!this.errors.Contains(columnName)) this.errors.Add(columnName);
                }

                return result;
            }
        }

        #endregion
    }
}
