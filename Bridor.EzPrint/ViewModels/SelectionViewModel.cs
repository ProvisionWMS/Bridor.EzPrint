using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using Bridor.EzPrint.Model;
using Bridor.EzPrint.Controls;
using Bridor.EzPrint.Helpers;
using Bridor.EzPrint.Resources;

namespace Bridor.EzPrint.ViewModels
{
    public class SelectionViewModel : INotifyPropertyChanged
    {
        #region -  Private Members  -
        /// <summary>
        /// Access to the data in the data store
        /// </summary>
        private Repository repo;
        /// <summary>
        /// Get or set  a value to indicate if the plant selection screen is to be shown
        /// </summary>
        private bool showPlantSelection = false;
        /// <summary>
        /// Value indicating the current page being displayed
        /// </summary>
        private Pages currentPage;
        /// <summary>
        /// Enum to select the error message to display
        /// </summary>
        private enum ErrorType { Database, NotFound };
        private ErrorType errorType;
        /// <summary>
        /// The current version of the application mayor.minor.build
        /// </summary>
        private string versionNumber = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
        #endregion

        #region -  Properties  -
        /// <summary>
        /// Get or set the visibility of the Menu Button
        /// </summary>
        public Visibility ShowMenuButton {
            get { return this.showMenuButton; }
            set {
                if (this.showMenuButton != value) {
                    this.showMenuButton = value;
                    OnPropertyChange("ShowMenuButton");
                }
            }
        }
        private Visibility showMenuButton;
        /// <summary>
        /// Get or set the visibility of the Exit Button
        /// </summary>
        public Visibility ShowExitButton {
            get { return this.showExitButton; }
            set {
                if (this.showExitButton != value) {
                    this.showExitButton = value;
                    OnPropertyChange("ShowExitButton");
                }
            }
        }
        private Visibility showExitButton;
        /// <summary>
        /// Get or set the windows's title 
        /// </summary>
        public string Title {
            get { return this.title; }
            set {
                if (this.title.CompareTo(value) != 0) {
                    this.title = value;
                    OnPropertyChange("Title");
                }
            }
        }
        private string title = String.Empty;
        /// <summary>
        /// Get or set the window's header
        /// </summary>
        public string Header {
            get { return this.header; }
            set {
                if (this.header.CompareTo(value) != 0) {
                    this.header = value;
                    OnPropertyChange("Header");
                }
            }
        }
        private string header = String.Empty;
        /// <summary>
        /// Get or set the text to display on the selection button
        /// </summary>
        public string SelectButtonText {
            get { return this.selectButtonText; }
            set {
                if (this.selectButtonText.CompareTo(value) != 0) {
                    this.selectButtonText = value;
                    OnPropertyChange("SelectButtonText");
                }
            }
        }
        private string selectButtonText = String.Empty;
        /// <summary>
        /// Get or set the selected plant
        /// </summary>
        public Plant Plant {
            get { return this.plant; }
            set {
                this.plant = value;
                try {
                    this.repo.LoadProductionLines(this.plant);
                } catch (BridorDatabaseException) {
                    DisplayError(ErrorType.Database);
                }
                OnPropertyChange("Plant");
            }
        }
        private Plant plant;
        /// <summary>
        /// Get or set the selected production line
        /// </summary>
        public ProductionLine ProductionLine {
            get { return this.productionLine; }
            set {
                this.productionLine = value;
                try {
                    this.repo.LoadTemplates(this.productionLine);
                } catch (BridorDatabaseException) {
                    DisplayError(ErrorType.Database);
                }
                OnPropertyChange("ProductionLine");
            }
        }
        private ProductionLine productionLine;
        /// <summary>
        /// Get or set the selected template
        /// </summary>
        public Template Template {
            get { return this.template; }
            set {
                this.template = value;
                try {
                    this.repo.LoadProducts(this.template);
                } catch (BridorDatabaseException) {
                    DisplayError(ErrorType.Database);
                }
                OnPropertyChange("Template");
            }
        }
        private Template template;
        /// <summary>
        /// Get or set the selected product
        /// </summary>
        public Product Product {
            get { return this.product; }
            set {
                this.product = value;
                OnPropertyChange("Product");
            }
        }
        private Product product;
        /// <summary>
        /// Get or set the visibility for the error message
        /// </summary>
        public Visibility ShowError {
            get { return this.showError; }
            set {
                if (this.showError != value) {
                    this.showError = value;
                    OnPropertyChange("ShowError");
                }
            }
        }
        private Visibility showError = Visibility.Collapsed;
        /// <summary>
        /// Get or set the error message to display
        /// </summary>
        public string ErrorMessage {
            get { return this.errorMessage; }
            set {
                if (String.IsNullOrEmpty(this.errorMessage) || this.errorMessage.CompareTo(value) != 0) {
                    this.errorMessage = value;
                    OnPropertyChange("ErrorMessage");
                }
            }
        }
        private string errorMessage;
        /// <summary>
        /// Get 
        /// </summary>
        public LabelData Label { get; private set; }
        /// <summary>
        /// Get or set the colleciton of plants
        /// </summary>
        public ObservableCollection<Plant> Plants { get; set; }
        /// <summary>
        /// Get the current content control
        /// </summary>
        public ContentControl CurrentControl { get; private set; }
        #endregion

        #region -  Commands  -
        /// <summary>
        /// Returns a value to determine if the process command can be applied
        /// </summary>
        private bool CanProcessCommand {
            get {
                switch (this.currentPage) {
                    case Pages.Plants:
                        return this.Plant != null;
                    case Pages.Lines:
                        return this.ProductionLine != null;
                    case Pages.Templates:
                        return this.Template != null;
                    case Pages.Products:
                        return this.Product != null;
                    case Pages.Product:
                        return this.Label.Error.CompareTo(String.Empty) == 0;
                    default:
                        return false;
                }
            }
        }
        /// <summary>
        /// Command to start process the current page
        /// </summary>
        public ICommand ProcessCommand {
            get {
                if (this.processCommand == null) {
                    this.processCommand = new RelayCommand(param => this.ProcessPage(), param => this.CanProcessCommand);
                }
                return this.processCommand;
            }
        }
        private RelayCommand processCommand;
        /// <summary>
        /// Command to process the return to the previous screen
        /// </summary>
        public ICommand MenuCommand {
            get {
                if (this.menuCommand == null) {
                    this.menuCommand = new RelayCommand(param => this.ProcessMenu(), param => true);
                }
                return this.menuCommand;
            }
        }
        private RelayCommand menuCommand;
        /// <summary>
        /// Command to process the return to the previous screen
        /// </summary>
        public ICommand HideErrorCommand {
            get {
                if (this.hideErrorCommand == null) {
                    this.hideErrorCommand = new RelayCommand(param => this.HideError(), param => true);
                }
                return this.hideErrorCommand;
            }
        }
        private RelayCommand hideErrorCommand;
        #endregion

        #region -  Constructor  -
        /// <summary>
        /// Initialzied the view model
        /// </summary>
        public SelectionViewModel() {
            // Initialize the repository
            this.repo = new Repository();
            // Process the startup
            ProcessStartUp();
        }
        #endregion

        #region -  Private Methods  -
        /// <summary>
        /// Process the start up of the application, and 
        /// displays the proper page
        /// </summary>
        private void ProcessStartUp() {
            // Set the current page to display
            if (Properties.Settings.Default.PlantNumber == 0) {
                // We need to display the plant selection
                this.currentPage = Pages.Plants;
                this.showPlantSelection = true;
            } else {
                // Set the default plant
                try {
                    this.Plant = this.repo.GetDefaultPlant(Properties.Settings.Default.PlantNumber);
                } catch (BridorDatabaseException) {
                    DisplayError(ErrorType.Database);
                }
                if (this.Plant == null) {
                    // The default plant does not exists.  Display the plant selection
                    this.currentPage = Pages.Plants;
                    this.showPlantSelection = true;
                } else {
                    // The default plant is valid.  Display the line selection and ignore plant selection
                    this.currentPage = Pages.Lines;
                    this.showPlantSelection = false;
                }
            }
            // Set the default values for the label data
            // CreateDefaultLabel();
            // Show the current page
            DisplayPage();
        }
        /// <summary>
        /// Sets the proper values to display the current page
        /// </summary>
        private void DisplayPage() {
            switch (currentPage) {
                case Pages.Plants:
                    try {
                        this.Plants = this.repo.GetPlants();
                    } catch (BridorDatabaseException) {
                        DisplayError(ErrorType.Database);
                    }
                    this.CurrentControl = new PlantSelectionControl();
                    this.ShowMenuButton = Visibility.Collapsed;
                    this.ShowExitButton = Visibility.Visible;
                    this.SelectButtonText = UIResources.PlantSelectionButton;
                    this.Header = UIResources.PlantSelectionHeader;
                    this.Title = String.Format("Bridor - {0} v{1}", UIResources.PlantSelectionHeader, versionNumber);
                    OnPropertyChange("Plants");
                    OnPropertyChange("CurrentControl");
                    break;
                case Pages.Lines:
                    // Reload the production lines
                    this.repo.LoadProductionLines(this.Plant);
                    this.CurrentControl = new LineSelectionControl();
                    if (this.showPlantSelection) {
                        this.ShowMenuButton = Visibility.Visible;
                        this.ShowExitButton = Visibility.Collapsed;
                    } else {
                        this.ShowMenuButton = Visibility.Collapsed;
                        this.ShowExitButton = Visibility.Visible;
                    }
                    this.SelectButtonText = UIResources.LineSelectionButton;
                    this.Header = UIResources.LineSelectionHeader;
                    this.Title = String.Format("Bridor - {0} v{1}", UIResources.LineSelectionHeader, versionNumber);
                    OnPropertyChange("CurrentControl");
                    break;
                case Pages.Templates:
                    this.CurrentControl = new TemplateSelectionControl();
                    this.ShowMenuButton = Visibility.Visible;
                    this.ShowExitButton = Visibility.Collapsed;
                    this.SelectButtonText = UIResources.TemplateSelectionButton;
                    this.Header = UIResources.TemplateSelectionHeader;
                    this.Title = String.Format("Bridor - {0} v{1}", UIResources.TemplateSelectionHeader, versionNumber);
                    OnPropertyChange("CurrentControl");
                    break;
                case Pages.Products:
                    this.CurrentControl = new ProductSelectionControl();
                    this.ShowMenuButton = Visibility.Visible;
                    this.ShowExitButton = Visibility.Collapsed;
                    this.SelectButtonText = UIResources.ProductSelectionButton;
                    this.Header = UIResources.ProductSelectionHeader;
                    this.Title = String.Format("Bridor - {0} v{1}", UIResources.ProductSelectionHeader, versionNumber);
                    OnPropertyChange("CurrentControl");
                    break;
                case Pages.Product:
                    this.CurrentControl = new EditProductControl();
                    this.ShowMenuButton = Visibility.Visible;
                    this.ShowExitButton = Visibility.Collapsed;
                    this.SelectButtonText = UIResources.ProductSelectionButton;
                    this.Header = UIResources.EditProductHeader;
                    this.Title = String.Format("Bridor - {0} v{1}", UIResources.EditProductHeader, versionNumber);
                    OnPropertyChange("CurrentControl");
                    break;
            }
        }
        /// <summary>
        /// Moves the users to the next page
        /// </summary>
        private void ProcessPage() {
            switch (this.currentPage) {
                case Pages.Plants:
                    // Set the current page to the production lines
                    this.currentPage = Pages.Lines;
                    break;
                case Pages.Lines:
                    // Set the current page to the templates
                    this.currentPage = Pages.Templates;
                    break;
                case Pages.Templates:
                    if (!this.Template.IsProductAcitve) {
                        CreateDefaultLabel(false);
                        this.currentPage = Pages.Product;
                    } else {
                        if (this.Template.Products.Count > 0) {
                            // Set the current page to the products
                            this.currentPage = Pages.Products;
                        } else {
                            DisplayError(ErrorType.NotFound);
                            // MessageBox.Show("Aucun produit associé à la TEMPLATE", "BRIDOR", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    break;
                case Pages.Products:
                    // Set the values for the label
                    CreateDefaultLabel(true);
                    // Display the product editing control
                    this.currentPage = Pages.Product;
                    break;
                case Pages.Product:
                    string sListOfLines="";
                    string message = "";
                    string printerIP = "";

                    //2024-03-21 add by JB ******************************************
                    if (Properties.Settings.Default.PrinterClock == true)
                    {
                        printerIP = this.repo.GetPrinterIPAddresses(this.productionLine.Name);
                        if (printerIP.Length > 0)
                        {
                            SendTimeAdjustCommand(printerIP);
                            LogWriter.Instance.Message("TimeAdjustCommand Sent", "IP:" + printerIP);
                        }
                        else
                        {
                            LogWriter.Instance.Error("TimeAdjustCommand", "IP not specified");
                        }
                    }
                   //*****************************************************************
                    List<string> IPAddresses = null;
                    //Send Clear Memory command to printers
                    try
                    {
                        sListOfLines = this.repo.GetListOfLines(this.productionLine.Id);
                        LogWriter.Instance.Message("ClearCommand Lines", sListOfLines);
                        if (sListOfLines.Length > 0)
                        {
                            IPAddresses = this.repo.GetListOfIPAddresses(sListOfLines);
                            if (IPAddresses.Count > 0)
                            {
                                foreach (string ip in IPAddresses)
                                {
                                    if (PingClient(ip) == true)
                                    {
                                        SendClearCommand(ip);
                                        LogWriter.Instance.Message("ClearCommand Sent", "IP:" + ip);
                                        
                                    }
                                    else
                                    {
                                        LogWriter.Instance.Error("SendingClearCommand", "Client: " + ip + " unreachable!");
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogWriter.Instance.Error("SendingClearCommand", ex.Message);
                    }

                    //2021-05-27
                    if (this.Template.IsWorkOrderNumberActive == true)
                    {
                        if (Properties.Settings.Default.BF_BARCODE == true)
                        {
                            if (this.Template.IsBarcodeOK == false)
                            {
                                if (Properties.Settings.Default.Language.ToUpper().CompareTo("ANGLAIS") == 0)
                                {
                                    message = "Invalid Shop Order / Manual Entry Not Permitted";
                                }
                                else
                                {
                                    message = "Bon Fabrication Non Valide / Entrée Manuelle Non Permise";
                                }
                                MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                                this.Label.WorkOrderNumberStr = "";
                                return;
                            }
                            else
                            {
                                //int iWoNumber;
                                long iWoNumber; //changed 2023-08-28 by JB
                                string result = "";
                                try
                                {
                                    iWoNumber = Convert.ToInt64(this.Label.WorkOrderNumberStr);
                                }
                                catch
                                {
                                    if (Properties.Settings.Default.Language.ToUpper().CompareTo("ANGLAIS") == 0)
                                        message = "Invalid Shop Order / Manual Entry Not Permitted";
                                    else
                                        message = "Bon Fabrication Non Valide / Entrée Manuelle Non Permise";

                                    MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    this.Label.WorkOrderNumberStr = "";
                                    result = UIResources.ValidationErrorWorkOrder;
                                    return;
                                }


                                if (this.Template.IsWorkOrderNumberActive && (this.Label.WorkOrderNumberStr.Length == 0 || (iWoNumber < 100000 || iWoNumber > 200000)))
                                {
                                    // WorkOrder number value is invalid
                                    if (iWoNumber < 1000000 || iWoNumber > 3000000)
                                    {
                                        if (iWoNumber != 999999)
                                        {
                                            if (Properties.Settings.Default.Language.ToUpper().CompareTo("ANGLAIS") == 0)
                                                message = "Invalid Shop Order / Manual Entry Not Permitted";
                                            else
                                                message = "Bon Fabrication Non Valide / Entrée Manuelle Non Permise";

                                            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                                            this.Label.WorkOrderNumberStr = "";
                                            result = UIResources.ValidationErrorWorkOrder;
                                            return;
                                        }
                                    }
                                }
                            }

                        }
                    }
                    
                    string id = this.Template.Id.ToString();
                    if (this.Template.ParentFormatName.EndsWith(".BTW") == true)
                    {

                        //Write command line file for Bartender
                        WriteCommandFileBartender();
                    }
                    else
                    {
                        //Write command line for Labelview
                        WriteCommandFile();
                    }
                    // Reset the selection
                    this.Template.IsBarcodeOK = false;
                    this.productionLine = null;
                    this.template = null;
                    this.product = null;
                    
                    // Set the current page
                    this.currentPage = Pages.Lines;
                    break;
            }
            // Display the selected page
            DisplayPage();
        }
        /// <summary>
        /// Returns to the main menu
        /// </summary>
        private void ProcessMenu() {
            switch (this.currentPage) {
                case Pages.Lines:
                    this.plant = null;
                    // Set the current page to the plant selection
                    this.currentPage = Pages.Plants;
                    break;
                case Pages.Templates:
                case Pages.Products:
                case Pages.Product:
                    this.productionLine = null;
                    // Reset the selection
                    this.template = null;
                    this.product = null;
                    // Set the current page to the Production Lines selection
                    this.currentPage = Pages.Lines;
                    break;
            }
            // Display the new selected page
            DisplayPage();
        }
        /// <summary>
        /// Writes the selected values to the command file for LabelView
        /// </summary>
        private void WriteCommandFile() {
            // Create the string for the first command file
            StringBuilder command = new StringBuilder();
            command.AppendFormat("LABELNAME={0}{1}\r\n", Properties.Settings.Default.PathLabels, this.Template.ParentFormatName);
            command.AppendFormat("PRINTER=\"{0}\"\r\n", this.ProductionLine.PrinterName);
            command.AppendFormat("PRODUIT=\"{0}\"\r\n", !String.IsNullOrEmpty(this.Label.ProductNumber) ? this.Label.ProductNumber : String.Empty);
            command.AppendFormat("BACKDAY=\"{0}\"\r\n", (this.Label.Backday > 0) ? this.Label.Backday.ToString() : String.Empty);
            command.AppendFormat("DESCRIPTION=\"{0}\"\r\n", !String.IsNullOrEmpty(this.Label.Description) ? this.Label.Description : String.Empty);
            //command.AppendFormat("BF=\"{0}\"\r\n", (this.Label.WorkOrderNumber.HasValue) ? this.Label.WorkOrderNumber.ToString() : String.Empty);
            //command.AppendFormat("BONFAB=\"{0}\"\r\n", (this.Label.WorkOrderNumber.HasValue) ? this.Label.WorkOrderNumber.ToString() : String.Empty);
            command.AppendFormat("BONFAB=\"{0}\"\r\n", this.Label.WorkOrderNumberStr.TrimEnd());
            command.AppendFormat("DATE_EXPIRATION=\"{0}\"\r\n", (this.Label.ExipryDate.HasValue) ? this.Label.ExipryDate.Value.ToString("dd-MM-yyyy") : String.Empty);
            command.AppendFormat("LIGNE_PRODUCTION=\"{0}\"\r\n", !String.IsNullOrEmpty(this.Label.ProductionLine) ? this.Label.ProductionLine : String.Empty);
            command.AppendFormat("LABELQUANTITY={0}\r\n", this.Label.Quantity);
            try {
                File.WriteAllText(Path.Combine(Properties.Settings.Default.PathCommand,
                    String.Format("{0}.cmd", DateTime.Now.ToString("yyyyMMddHHmmss"))),
                    command.ToString());
            } catch (Exception exc) {
                LogWriter.Instance.Error("SelectionViewModel.WriteCommandFile-Parent", exc.Message);
            }
#if DEBUG
            MessageBox.Show(command.ToString());
#endif
            if (!String.IsNullOrEmpty(this.Template.ChildFormatName)) {
                // Create the child label command file
                command.Clear();
                command.AppendFormat("LABELNAME={0}{1}\r\n", Properties.Settings.Default.PathLabels, this.Template.ChildFormatName);
                command.AppendFormat("PRINTER=\"{0}\"\r\n", this.ProductionLine.PrinterName);
                command.AppendFormat("PRODUIT=\"{0}\"\r\n", !String.IsNullOrEmpty(this.Label.ProductNumber) ? this.Label.ProductNumber : String.Empty);
                command.AppendFormat("BACKDAY=\"{0}\"\r\n", (this.Label.Backday > 0) ? this.Label.Backday.ToString() : String.Empty);
                command.AppendFormat("DESCRIPTION=\"{0}\"\r\n", !String.IsNullOrEmpty(this.Label.Description) ? this.Label.Description : String.Empty);
                //command.AppendFormat("BF=\"{0}\"\r\n", (this.Label.WorkOrderNumber.HasValue) ? this.Label.WorkOrderNumber.ToString() : String.Empty);
                //command.AppendFormat("BONFAB=\"{0}\"\r\n", (this.Label.WorkOrderNumber.HasValue) ? this.Label.WorkOrderNumber.ToString() : String.Empty); 
                command.AppendFormat("BONFAB=\"{0}\"\r\n", this.Label.WorkOrderNumberStr); 
                command.AppendFormat("DATE_EXPIRATION=\"{0}\"\r\n", (this.Label.ExipryDate.HasValue) ? this.Label.ExipryDate.Value.ToString("dd-MM-yyyy") : String.Empty);
                command.AppendFormat("LIGNE_PRODUCTION=\"{0}\"\r\n", !String.IsNullOrEmpty(this.Label.ProductionLine) ? this.Label.ProductionLine : String.Empty);
                command.AppendFormat("LABELQUANTITY={0}\r\n", this.Label.Quantity);
                try {
                    File.WriteAllText(Path.Combine(Properties.Settings.Default.PathCommand,
                        String.Format("{0}1.cmd", DateTime.Now.ToString("yyyyMMddHHmmss"))),
                        command.ToString());
                } catch (Exception exc) {
                    LogWriter.Instance.Error("SelectionViewModel.WriteCommandFile-Child", exc.Message);
                }
#if DEBUG
                MessageBox.Show(command.ToString());
#endif
            }
        }

        private void WriteCommandFileBartender()
        {
            // Create the string for the first command file
            StringBuilder command = new StringBuilder();
            command.AppendFormat("Label_Name,Printer,Produit,Backday,Description,BF,Date_Expiration,Ligne_Production,LabelQuantity\r\n");
            command.AppendFormat("{0},{1},",this.Template.ParentFormatName, this.ProductionLine.PrinterName);
            command.AppendFormat("{0},", !String.IsNullOrEmpty(this.Label.ProductNumber) ? this.Label.ProductNumber : String.Empty);
            //command.AppendFormat("{0},", (this.Label.Backday > 0) ? this.Label.Backday.ToString() : String.Empty);
            command.AppendFormat("{0},", (this.Label.Backday.ToString()));
            command.AppendFormat("{0},", !String.IsNullOrEmpty(this.Label.Description) ? this.Label.Description : String.Empty);
            //command.AppendFormat("BF=\"{0}\"\r\n", (this.Label.WorkOrderNumber.HasValue) ? this.Label.WorkOrderNumber.ToString() : String.Empty);
            //command.AppendFormat("{0},", (this.Label.WorkOrderNumber.HasValue) ? this.Label.WorkOrderNumber.ToString() : String.Empty);
            command.AppendFormat("{0},", (this.Label.WorkOrderNumberStr.TrimEnd()));
            command.AppendFormat("{0},", (this.Label.ExipryDate.HasValue) ? this.Label.ExipryDate.Value.ToString("dd-MM-yyyy") : String.Empty);
            command.AppendFormat("{0},", !String.IsNullOrEmpty(this.Label.ProductionLine) ? this.Label.ProductionLine : String.Empty);
            command.AppendFormat("{0}\r\n", this.Label.Quantity);
            try
            {
                File.WriteAllText(Path.Combine(Properties.Settings.Default.PathCommandBartender,
                    String.Format("{0}.csv", DateTime.Now.ToString("yyyyMMddHHmmss"))),
                    command.ToString());
            }
            catch (Exception exc)
            {
                LogWriter.Instance.Error("SelectionViewModel.WriteCommandFileBartender", exc.Message);
            }
#if DEBUG
            MessageBox.Show(command.ToString());
#endif
        }
        /// <summary>
        /// Create the default label data
        /// </summary>
        private void CreateDefaultLabel(bool haveProduct) {
            // Set the current product to null
            if(!haveProduct) this.Product = null;
            // Create a new label for the command file processing
            this.Label = new LabelData();
            this.Label.Backday = 0;
            this.Label.Quantity = 1;
            this.Label.Template = this.Template;
            this.Label.ProductionLine = String.Empty;

            if (this.Product == null) {
                // The product is null, The user will have the opportunity to update this values according
                // to their active flag
                if (this.Template != null) {
                    this.Label.WorkOrderNumberStr = String.Empty;
                    this.Label.Description = String.Empty;
                    this.Label.ProductNumber = String.Empty;
                    this.Label.ExipryDate = null;
                }
            } else {
                // Put values that will leave this fields blank on the command file
                this.Label.ProductNumber = this.Product.ProductNumber;
                this.Label.WorkOrderNumberStr = String.Empty;
                this.Label.Description = null;
                this.Label.ExipryDate = null;
            }
        }
        /// <summary>
        /// Display the error message to the user
        /// </summary>
        /// <param name="type">What type of error are we displaying</param>
        private void DisplayError(ErrorType type) {
            if (type == ErrorType.Database) {
                this.ErrorMessage = UIResources.DatabaseErrorMessage;
            } else if (type == ErrorType.NotFound) {
                this.ErrorMessage = UIResources.ProductNotFoundMessage;
            }
            this.errorType = type;
            this.ShowError = Visibility.Visible;
        }
        /// <summary>
        /// Hides the error message when the user ackowledges it
        /// </summary>
        private void HideError() {
            this.ErrorMessage = String.Empty;
            this.ShowError = Visibility.Collapsed;
            if (this.errorType == ErrorType.Database) {
                // Try to reinitialize the applicaiton
                ProcessStartUp();
            }
        }

        private void SendClearCommand(String clientIP)
        {
            try
            {
                Int32 port = 9100;
                TcpClient client = new TcpClient(clientIP, port);

                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes("~JA");

                // Get a client stream for reading and writing.
                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);

                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                LogWriter.Instance.Error("SendClearCommand-General Exception", e.Message);
            }
            catch (SocketException e)
            {
                LogWriter.Instance.Error("SendClearCommand-Socket Exception", e.Message);
            }
        }

        //2024-03-21 add by JB
        private void SendTimeAdjustCommand(String clientIP)
        {
            try
            {
                Int32 port = 9100;
                TcpClient client = new TcpClient(clientIP, port);
                string dateAdjustCommand = "";

                dateAdjustCommand = "^XA^ST" + DateTime.Now.ToString("MM" + "," + "dd" + "," + "yyyy" + "," + "HH" + "," + "mm" + "," + "00") + ",M^FS^XZ";

                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(dateAdjustCommand);

                // Get a client stream for reading and writing.
                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);

                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                LogWriter.Instance.Error("SendTimeAdjustCommand-General Exception", e.Message);
            }
            catch (SocketException e)
            {
                LogWriter.Instance.Error("SendTimeAdjustCommand-Socket Exception", e.Message);
            }
        }
        private bool PingClient(String clientIP)
        {
            bool isValid = false;

            Ping p = new Ping();
            PingReply r;
            r = p.Send(clientIP,2000);

            if (r.Status == IPStatus.Success)
            {
                isValid = true;
            }
            return isValid;
        }
        #endregion

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
    }
}
