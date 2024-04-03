using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bridor.EzPrint.Resources
{
    public static class UIResources
    {
        #region -  Private Members  -
        /// <summary>
        /// Value to indicate if the current language is Englis
        /// </summary>
        private static bool IsEnglis = Properties.Settings.Default.Language.ToUpper().CompareTo("ANGLAIS") == 0;
        #endregion

        #region -  Main Window Buttons  -
        public static string PlantLabel {
            get {
                return (IsEnglis) ? "SHOP: " : "USINE: ";
            }
        }
        public static string LineLabel {
            get {
                return (IsEnglis) ? "          LINE: " : "          LIGNE: ";
            }
        }
        public static string ExitButton {
            get {
                return (IsEnglis) ? "Exit" : "Quitter";
            }
        }
        public static string ReturnButton {
            get {
                return (IsEnglis) ? "Main\r\nMenu" : "Menu\r\nPrincipal";
            }
        }
        public static string DatabaseErrorMessage {
            get {
                return (IsEnglis) ? "Database error" : "Erreur de base de données";
            }
        }
        public static string ProductNotFoundMessage {
            get {
                return (IsEnglis) ? "No products associated with the TEMPLATE" : "Aucun produit associé à la TEMPLATE";
            }
        }
        #endregion
        
        #region -  Plant Selection  -
        public static string PlantSelectionHeader {
            get {
                return (IsEnglis) ? "Shop selection" : "Sélection de l'usine";
            }
        }
        public static string PlantSelectionListHeader {
            get {
                return (IsEnglis) ? "SHOPS" : "USINES";
            }
        }
        public static string PlantSelectionDescription {
            get {
                return (IsEnglis)
                    ? "Choose a shop then click on the Select Production Line button"
                    : "Sélectionner une usine et appuyer sur le bouton Choisir Ligne Production";
            }
        }
        public static string PlantSelectionButton {
            get {
                return (IsEnglis) ? "Select\r\nProduction Line" : "Choisir Ligne\r\nProduction";
            }
        }
        #endregion

        #region -  Line Selection  -
        public static string LineSelectionHeader {
            get {
                return (IsEnglis) ? "Production line selection" : "Sélection de la ligne de production";
            }
        }
        public static string LineSelectionListHeader {
            get {
                return (IsEnglis) ? "PRODUCTION LINES" : "LIGNES DE PRODUCTION";
            }
        }
        public static string LineSelectionDescription {
            get {
                return (IsEnglis)
                    ? "Choose a production line then click on the Select Template button"
                    : "Sélectionner une ligne de production et appuyer sur le bouton Choisir Template";
            }
        }
        public static string LineSelectionButton {
            get {
                return (IsEnglis) ? "Select\r\nTemplate" : "Choisir\r\nTemplate";
            }
        }
        #endregion

        #region -  Template Selection  -
        public static string TemplateSelectionHeader {
            get {
                return (IsEnglis) ? "Template selection" : "Sélection de la Template";
            }
        }
        public static string TemplateSelectionListHeader {
            get {
                return (IsEnglis) ? "TEMPLATES" : "TEMPLATES";
            }
        }
        public static string TemplateSelectionDescription {
            get {
                return (IsEnglis)
                    ? "Choose a template then click on the Select Product button"
                    : "Sélectionner une template et appuyer sur le bouton Choisir Produit";
            }
        }
        public static string TemplateSelectionButton {
            get {
                return (IsEnglis) ? "Select\r\nProduct" : "Choisir\r\nProduit";
            }
        }
        #endregion

        #region -  Product Selection  -
        public static string ProductSelectionHeader {
            get {
                return (IsEnglis) ? "Product selection" : "Sélection du Produit";
            }
        }
        public static string ProductSelectionProductColumn {
            get {
                return (IsEnglis) ? "Product" : "Produit";
            }
        }
        public static string ProductSelectionBrandColumn {
            get {
                return (IsEnglis) ? "Brand" : "Marque";
            }
        }
        public static string ProductSelectionDescription {
            get {
                return (IsEnglis)
                    ? "Choose a product then click on the Print button"
                    : "Sélectionner un produit et appuyer sur le bouton Lancer Impression";
            }
        }
        public static string ProductSelectionButton {
            get {
                return (IsEnglis) ? "Print" : "Lancer\r\nImpression";
            }
        }
        #endregion

        #region -  Edit Product  -
        public static string EditProductHeader {
            get {
                return (IsEnglis) ? "Confirm data to print" : "Confirmer Données Impression";
            }
        }
        public static string ProductLabel {
            get {
                return (IsEnglis) ? "PRODUCT: " : "PRODUIT: ";
            }
        }
        public static string BrandLabel {
            get {
                return (IsEnglis) ? "          BRAND: " : "          MARQUE: ";
            }
        }
        public static string NewProductLabel {
            get {
                return (IsEnglis) ? "NEW PRODUCT:" : "NOUVEAU PRODUIT:";
            }
        }
        public static string QuantityLabel {
            get {
                return (IsEnglis) ? "QUANTITY:" : "QUANTITÉ:";
            }
        }
        public static string WorkOrderLabel {
            get {
                return (IsEnglis) ? "SHOP ORDER:" : "BON FABRICATION (BF):";
            }
        }
        public static string ProductionLineLabel {
            get {
                return (IsEnglis) ? "PRODUCTION LINE:" : "LIGNE PRODUCTION:";
            }
        }
        public static string ExpiryDateLabel {
            get {
                return (IsEnglis) ? "EXPIRY DATE:" : "DATE EXPIRATION:";
            }
        }
        #endregion

        #region -  Validation Errors  -
        public static string ValidationErrorQuantity {
            get {
                return (IsEnglis) ? "The maximum quantity is {0}" : "La quantité maximale est {0}";
            }
        }
        public static string ValidationErrorBackday {
            get {
                return (IsEnglis) ? "Possible values for Backay - 0 to 360" : "Valeur possible du Backday - 0 a 360";
            }
        }
        public static string ValidationErrorWorkOrder {
            get {
                return (IsEnglis) ? "Possible values for Shop Order - 100,000 to 200,000 or 1 000 000 to 3 000 000" : "Valeur possible du Bon Fabrication - 100 000 a 200 000 ou 1 000 000 a 3 000 000";
            }
        }
        public static string ValidationErrorProductNumber {
            get {
                return (IsEnglis) ? "Product Number is required" : "Code de Produit requis";
            }
        }
        public static string ValidationErrorProductionLine {
            get {
                return (IsEnglis) ? "Production Line is required" : "Ligne de production requise";
            }
        }
        public static string ValidationErrorDescription {
            get {
                return (IsEnglis) ? "Description is required" : "Description requise";
            }
        }
        public static string ValidationErrorDateRequired {
            get {
                return (IsEnglis) ? "Expiration Date is required" : "Date expiration requise";
            }
        }
        public static string ValidationErrorDateInvalid {
            get {
                return (IsEnglis) ? "The expiration date must be greater than today's date" : "La date d'expiration doit être supérieure à la date d'aujourd'hui";
            }
        }
        #endregion
    }
}
