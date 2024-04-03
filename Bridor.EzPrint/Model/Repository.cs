using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Bridor.EzPrint.Helpers;

namespace Bridor.EzPrint.Model
{
    public class Repository
    {
        #region -  Private Members  -
        /// <summary>
        /// Get the conneciton to the database
        /// </summary>
        private SqlConnection Connection {
            get { return new SqlConnection(Properties.Settings.Default.DataSource); }
        }
        #endregion

        #region -  Public Methods  -
        /// <summary>
        /// Get the list of plants for the user selection
        /// </summary>
        public ObservableCollection<Plant> GetPlants() {
            // Return empty colleciton if no records are retreived
            ObservableCollection<Plant> result = new ObservableCollection<Plant>();
            using (SqlConnection conn = this.Connection) {
                try {
                    string query = "SELECT ID_USINE, NOM_USINE FROM USINE";
                    SqlCommand command = new SqlCommand(query, conn);
                    conn.Open();
                    using (SqlDataReader dr = command.ExecuteReader()) {
                        if (dr != null && dr.HasRows) {
                            while (dr.Read()) {
                                Plant item = new Plant();
                                item.Id = Convert.ToInt32(dr["ID_USINE"]);
                                item.Name = dr["NOM_USINE"].ToString();
                                result.Add(item);
                            }
                        }
                    }
                } catch(SqlException exc) {
                    LogDatabaseException("Repository.GetPlants()", exc);
                } catch (Exception exc){
                    LogWriter.Instance.Error("Repository.GetPlants()", exc.Message);
                } finally {
                    if (conn.State == ConnectionState.Open) {
                        conn.Close();
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// Get the default plant
        /// </summary>
        /// <param name="planNumber">Number of the default plant</param>
        public Plant GetDefaultPlant(int planNumber) {
            // Assume failure
            Plant result = null;
            using (SqlConnection conn = this.Connection) {
                string query = "SELECT ID_USINE, NOM_USINE FROM dbo.USINE WHERE ID_USINE = @PlantNumber";
                try{
                    SqlCommand command = new SqlCommand(query, conn);
                    command.Parameters.AddWithValue("@PlantNumber", planNumber);
                    conn.Open();
                    using (SqlDataReader dr = command.ExecuteReader()) {
                        if (dr != null && dr.Read()) {
                            result = new Plant();
                            result.Id = Convert.ToInt32(dr["ID_USINE"]);
                            result.Name = dr["NOM_USINE"].ToString();
                        }
                    }
                    if (result != null) {
                        // Load the lines that belong to the default plant
                        LoadProductionLines(result);
                    }
                } catch(SqlException exc) {
                    LogDatabaseException("Repository.GetDefaultPlant()", exc);
                } catch(Exception exc) {
                    LogWriter.Instance.Error("Repository.GetDefaultPlant()", exc.Message);
                } finally {
                    if (conn.State == ConnectionState.Open) {
                        conn.Close();
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// Loads the production lines for the selected plant
        /// </summary>
        /// <param name="plant">Plant that requires the production lines</param>
        public void LoadProductionLines(Plant plant) {
            string query = "SELECT ID_LIGNE, NOM_LIGNE, IMPRIMANTE, ID_USINE FROM LIGNE WHERE ID_USINE = @PlantId";
            using (SqlConnection conn = this.Connection) {
                try {
                    SqlCommand command = new SqlCommand(query, conn);
                    command.Parameters.AddWithValue("@PlantId", plant.Id);
                    conn.Open();
                    using (SqlDataReader dr = command.ExecuteReader()) {
                        if (dr != null && dr.HasRows) {
                            // Clear any previous production lines
                            plant.ProductionLines.Clear();
                            while (dr.Read()) {
                                // Add the new production lines
                                ProductionLine item = new ProductionLine();
                                item.Id = Convert.ToInt32(dr["ID_LIGNE"]);
                                item.Name = dr["NOM_LIGNE"].ToString();
                                item.PrinterName = dr["IMPRIMANTE"].ToString();
                                item.Plant = plant;
                                plant.ProductionLines.Add(item);
                            }
                        }
                    }
                } catch (SqlException exc) {
                    LogDatabaseException("Repository.LoadProductionLines()", exc);
                } catch (Exception exc) {
                    LogWriter.Instance.Error("Repository.LoadProductionLines()", exc.Message);
                } finally {
                    if (conn.State == ConnectionState.Open) {
                        conn.Close();
                    }
                }
            }
        }
        /// <summary>
        /// Loads the templates for the selected production line
        /// </summary>
        /// <param name="line">Production line we need the templates for</param>
        public void LoadTemplates(ProductionLine line) {
            string query = @"SELECT ID_TEMPLATE,TEMPLATE_DEVANT,TEMPLATE_COTE,DESCRIP_TEMPLATE,
                             PRODUIT,BACKDAY,QUANTITE,DESCRIPTION_NEW,BF,PRODUIT_NEW,DATE_EXPIRATION,LIGNE_PRODUCTION
                             FROM dbo.TEMPLATE WHERE ID_LIGNE = @Line";
            using (SqlConnection conn = this.Connection) {
                try {
                    SqlCommand command = new SqlCommand(query, conn);
                    command.Parameters.AddWithValue("@Line", line.Id);
                    conn.Open();
                    using (SqlDataReader dr = command.ExecuteReader()) {
                        if (dr != null && dr.HasRows) {
                            // Clear any previous templates
                            line.Templates.Clear();
                            while (dr.Read()) {
                                // Add the new Template
                                Template item = new Template();
                                item.Id = Convert.ToInt32(dr["ID_TEMPLATE"]);
                                item.ParentFormatName = dr["TEMPLATE_DEVANT"].ToString().ToUpper();
                                item.ChildFormatName = dr["TEMPLATE_COTE"].ToString();
                                item.Description = dr["DESCRIP_TEMPLATE"].ToString();
                                item.IsProductAcitve = Convert.ToBoolean(dr["PRODUIT"]);
                                item.IsBackdayActive = Convert.ToBoolean(dr["BACKDAY"]);
                                item.IsQuantityActive = Convert.ToBoolean(dr["QUANTITE"]);
                                item.IsNewDescriptionActive = Convert.ToBoolean(dr["DESCRIPTION_NEW"]);
                                //changed 2021-05-27 by Jerry
                                //if (Properties.Settings.Default.BF_REQUIRED == true)
                                //    item.IsWorkOrderNumberActive = true;
                                //else
                                //    item.IsWorkOrderNumberActive = Convert.ToBoolean(dr["BF"]);

                                //Changed 2021-10-07 by Jerry
                                //if (Properties.Settings.Default.BF_REQUIRED == true)
                                //    item.IsWorkOrderNumberActive = true;
                                //else
                                item.IsWorkOrderNumberActive = Convert.ToBoolean(dr["BF"]);

                                item.IsNewProductActive = Convert.ToBoolean(dr["PRODUIT_NEW"]);
                                item.IsExpiryDateActive = Convert.ToBoolean(dr["DATE_EXPIRATION"]);
                                item.IsOriginalProductionLineActive = Convert.ToBoolean(dr["LIGNE_PRODUCTION"]);
                                item.IsBarcodeOK = false; //add 2021-05-27
                                item.Line = line;
                                line.Templates.Add(item);
                            }
                        }
                    }
                } catch (SqlException exc) {
                    LogDatabaseException("Repository.LoadTemplates()", exc);
                } catch (Exception exc) {
                    LogWriter.Instance.Error("Repository.LoadTemplates()", exc.Message);
                } finally {
                    if (conn.State == ConnectionState.Open) {
                        conn.Close();
                    }
                }
            }
        }
        /// <summary>
        /// Loads the products that user the selected template
        /// </summary>
        /// <param name="template">Template being used by the products</param>
        public void LoadProducts(Template template) {
            //expanded statemtn by addting num 11-15 on 2022-10-03
            string query = @"SELECT ID_ETIQUETTE,PRODUIT,DESC_FR,DESC_EN,MARQUE FROM ETIQUETTE
                             WHERE (ID_TEMPLATE_1 = @TemplateId OR ID_TEMPLATE_2 = @TemplateId
                             OR ID_TEMPLATE_3 = @TemplateId OR ID_TEMPLATE_4 = @TemplateId
                             OR ID_TEMPLATE_5 = @TemplateId OR ID_TEMPLATE_6 = @TemplateId
                             OR ID_TEMPLATE_7 = @TemplateId OR ID_TEMPLATE_8 = @TemplateId
                             OR ID_TEMPLATE_9 = @TemplateId OR ID_TEMPLATE_10 = @TemplateId
                             OR ID_TEMPLATE_11 = @TemplateId OR ID_TEMPLATE_12 = @TemplateId
                             OR ID_TEMPLATE_13 = @TemplateId OR ID_TEMPLATE_14 = @TemplateId
                             OR ID_TEMPLATE_15 = @TemplateId)
                             ORDER BY PRODUIT";
            using (SqlConnection conn = this.Connection) {
                try {
                    SqlCommand command = new SqlCommand(query, conn);
                    command.Parameters.AddWithValue("@TemplateId", template.Id);
                    conn.Open();
                    using (SqlDataReader dr = command.ExecuteReader()) {
                        if (dr != null && dr.HasRows) {
                            // Clear any previous templates
                            template.Products.Clear();
                            while (dr.Read()) {
                                // Add the new Template
                                Product item = new Product();
                                item.Id = Convert.ToInt32(dr["ID_ETIQUETTE"]);
                                item.ProductNumber = dr["PRODUIT"].ToString();
                                item.FrenchDescription = dr["DESC_FR"].ToString();
                                item.EnglishDescription = dr["DESC_EN"].ToString();
                                item.BrandName = dr["MARQUE"].ToString();
                                item.Template = template;
                                template.Products.Add(item);
                            }
                        }
                    }
                } catch (SqlException exc) {
                    LogDatabaseException("Repository.LoadProducts()", exc);
                } catch (Exception exc) {
                    LogWriter.Instance.Error("Repository.LoadProducts()", exc.Message);
                } finally {
                    if (conn.State == ConnectionState.Open) {
                        conn.Close();
                    }
                }
            }
        }
        #endregion

        #region -  Private Methods  -
        /// <summary>
        /// Checks if the SQL exception is a connection problem to notify user
        /// </summary>
        /// <param name="header">Source of the exception</param>
        /// <param name="exc">Exception thrown</param>
        private void LogDatabaseException(string header, SqlException exc) {
            switch (exc.Number) {
                case -1:
                case -2:
                case 2:
                case 53:
                case 4060:
                    LogWriter.Instance.ErrorFormat(header, "Database connection error - {0}", exc.Message);
                    throw new BridorDatabaseException();
                default:
                    LogWriter.Instance.Error(header, exc.Message);
                    break;
            }
        }

        public string GetListOfLines(int iLineId)
        {
            string result = "";

            string query = @"SELECT CLEAR_MEMORY FROM LIGNE WHERE ID_LIGNE =@lineid";

            using (SqlConnection conn = this.Connection)
            {
                try
                {
                    SqlCommand command = new SqlCommand(query, conn);
                    command.Parameters.AddWithValue("@lineid", iLineId);
                    conn.Open();
                    using (SqlDataReader dr = command.ExecuteReader())
                    {
                        if (dr != null && dr.HasRows)
                        {
                            if (dr.Read())
                            {
                                result = (dr["CLEAR_MEMORY"] != DBNull.Value) ? dr["CLEAR_MEMORY"].ToString() : "";
                            }
                        }
                    }
                }
                catch (SqlException exc)
                {
                    LogDatabaseException("Repository.GetListOfLines()", exc);
                }
                catch (Exception exc)
                {
                    LogWriter.Instance.Error("Repository.GetListOfLines()", exc.Message);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }
            return result;
        }

        public List<string> GetListOfIPAddresses(string sList)
        {
            List<string> result = null;

            string query = @"SELECT IP_ADDRESS FROM LIGNE WHERE ID_LIGNE IN (" + sList  + ") ";

            using (SqlConnection conn = this.Connection)
            {
                try
                {
                    SqlCommand command = new SqlCommand(query, conn);
                    //command.Parameters.AddWithValue("@ids", sList);
                    conn.Open();
                    using (SqlDataReader dr = command.ExecuteReader())
                    {
                        if (dr != null && dr.HasRows)
                        {
                            result = new List<string>();
                            while (dr.Read())
                            {
                                result.Add((dr["IP_ADDRESS"] != DBNull.Value) ? dr["IP_ADDRESS"].ToString() : "");
                            }
                        }
                    }
                }
                catch (SqlException exc)
                {
                    LogDatabaseException("Repository.GetListOfIPAddresses()", exc);
                }
                catch (Exception exc)
                {
                    LogWriter.Instance.Error("Repository.GetListOfIPAddresses()", exc.Message);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }
            return result;
        }

        public string GetPrinterIPAddresses(string sPrinter)
        {
            string result = null;

            string query = @"SELECT IP_ADDRESS FROM LIGNE WHERE NOM_LIGNE ='" + sPrinter +  "'";

            using (SqlConnection conn = this.Connection)
            {
                try
                {
                    SqlCommand command = new SqlCommand(query, conn);
                    //command.Parameters.AddWithValue("@ids", sList);
                    conn.Open();
                    using (SqlDataReader dr = command.ExecuteReader())
                    {
                        if (dr != null && dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                result= (dr["IP_ADDRESS"] != DBNull.Value) ? dr["IP_ADDRESS"].ToString() : "";
                            }
                        }
                    }
                }
                catch (SqlException exc)
                {
                    LogDatabaseException("Repository.GetPrinterIPAddresses()", exc);
                }
                catch (Exception exc)
                {
                    LogWriter.Instance.Error("Repository.GetPrinterIPAddresses()", exc.Message);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }
            return result;
        }
        #endregion
    }
}
