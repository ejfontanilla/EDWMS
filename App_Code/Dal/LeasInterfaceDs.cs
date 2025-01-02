using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.Web.Security;
using Dwms.Bll;

namespace Dwms.Dal
{
    public class LeasInterfceDs
    {
        static string connString =
            ConfigurationManager.ConnectionStrings["ASPNETDBConnectionString"].ToString();

        #region Retrieve Methods


        public static DataTable GetLeasInterfaceByHleNumberAndNricForPendingDocumentDisplay(string hleNumber, string nric)
        {
            SqlCommand command = new SqlCommand();
            StringBuilder sqlStatement = new StringBuilder();

            //retrieve info
            sqlStatement.Append("SELECT li.*,dt.Description, CASE WHEN (Category is NOT NULL AND LEN(Category)>0) THEN Category ELSE convert(varchar(200),Id) END  ");
            sqlStatement.Append("+CASE WHEN (CategoryGroup is NOT NULL AND LEN(CategoryGroup)>0) THEN CategoryGroup ELSE convert(varchar(200),Id) END  ");
            sqlStatement.Append("+COALESCE(CONVERT(VARCHAR(10),CategoryDate,110),'0') AS CategoryInfo, DocTypeCode as DocTypeCodeRevised ");
            sqlStatement.Append("FROM LeasInterface li, DocType dt ");
            sqlStatement.Append("WHERE li.DocTypeCode= dt.Code ");
            sqlStatement.Append("AND HleNumber=@HleNumber AND NRIC=@Nric ");
            sqlStatement.Append("ORDER BY CategoryInfo ");

            command.Parameters.Add("@HleNumber", SqlDbType.NVarChar);
            command.Parameters["@HleNumber"].Value = hleNumber;

            command.Parameters.Add("@Nric", SqlDbType.NVarChar);
            command.Parameters["@Nric"].Value = nric;

            using (SqlConnection connection = new SqlConnection(connString))
            {
                command.CommandText = sqlStatement.ToString();
                command.Connection = connection;
                DataSet dataSet = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dataSet);

                DataTable pendingDocuments = dataSet.Tables[0];

                if (pendingDocuments.Rows.Count == 0)
                {
                    return pendingDocuments;
                }
                else
                {
                    string categoryInfo = string.Empty;

                    int pendingDocumentsRevisedRowCount = 0;

                    DataTable pendingDocumentsRevised = new DataTable();
                    pendingDocumentsRevised.Columns.Add("DocTypeDescriptionRevised");
                    pendingDocumentsRevised.Columns.Add("DocStartDate");
                    pendingDocumentsRevised.Columns.Add("DocEndDate");
                    pendingDocumentsRevised.Columns.Add("StatusGroup");

                    for (int i = 0; i < pendingDocuments.Rows.Count; i++)
                    {
                        string status = string.IsNullOrEmpty(pendingDocuments.Rows[i]["Status"].ToString().Trim()) ? "CCC" : pendingDocuments.Rows[i]["Status"].ToString().Trim();

                        if (string.IsNullOrEmpty(categoryInfo))
                        {
                            categoryInfo = pendingDocuments.Rows[i]["CategoryInfo"].ToString();

                            DataRow pendingDocumentsRevisedRow = pendingDocumentsRevised.NewRow();
                            pendingDocumentsRevisedRow["DocTypeDescriptionRevised"] = pendingDocuments.Rows[i]["Description"].ToString();
                            pendingDocumentsRevisedRow["DocStartDate"] = pendingDocuments.Rows[i]["DocStartDate"].ToString();
                            pendingDocumentsRevisedRow["DocEndDate"] = pendingDocuments.Rows[i]["DocEndDate"].ToString();
                            pendingDocumentsRevisedRow["StatusGroup"] = status;
                                                        
                            pendingDocumentsRevised.Rows.Add(pendingDocumentsRevisedRow);
                            pendingDocumentsRevisedRowCount = pendingDocumentsRevisedRowCount + 1;
                        }
                        else
                        {
                            status = string.IsNullOrEmpty(pendingDocuments.Rows[i]["Status"].ToString().Trim()) ? "CCC" : pendingDocuments.Rows[i]["Status"].ToString().Trim();

                            if (categoryInfo == pendingDocuments.Rows[i]["CategoryInfo"].ToString())
                            {
                                if (!pendingDocumentsRevised.Rows[pendingDocumentsRevisedRowCount - 1]["StatusGroup"].ToString().Trim().ToUpper().Contains("C") && !status.Trim().ToUpper().Contains("C"))
                                {
                                    pendingDocumentsRevised.Rows[pendingDocumentsRevisedRowCount - 1].BeginEdit();
                                    pendingDocumentsRevised.Rows[pendingDocumentsRevisedRowCount - 1]["DocTypeDescriptionRevised"] += " <font class='operator'>(OR)</font> " + pendingDocuments.Rows[i]["Description"].ToString();
                                    pendingDocumentsRevised.Rows[pendingDocumentsRevisedRowCount - 1]["StatusGroup"] += status;
                                    pendingDocumentsRevised.Rows[pendingDocumentsRevisedRowCount - 1].AcceptChanges();
                                }
                                else if (!status.Trim().ToUpper().Contains("C"))//Added to show single either-or item(temp)
                                {
                                    DataRow pendingDocumentsRevisedRow = pendingDocumentsRevised.NewRow();
                                    pendingDocumentsRevisedRow["DocTypeDescriptionRevised"] = pendingDocuments.Rows[i]["Description"].ToString();
                                    pendingDocumentsRevisedRow["DocStartDate"] = pendingDocuments.Rows[i]["DocStartDate"].ToString();
                                    pendingDocumentsRevisedRow["DocEndDate"] = pendingDocuments.Rows[i]["DocEndDate"].ToString();
                                    pendingDocumentsRevisedRow["StatusGroup"] = status;

                                    pendingDocumentsRevised.Rows.Add(pendingDocumentsRevisedRow);
                                    pendingDocumentsRevisedRowCount = pendingDocumentsRevisedRowCount + 1;
                                }
                            }
                            else
                            {
                                DataRow pendingDocumentsRevisedRow = pendingDocumentsRevised.NewRow();
                                pendingDocumentsRevisedRow["DocTypeDescriptionRevised"] = pendingDocuments.Rows[i]["Description"].ToString();
                                pendingDocumentsRevisedRow["DocStartDate"] = pendingDocuments.Rows[i]["DocStartDate"].ToString();
                                pendingDocumentsRevisedRow["DocEndDate"] = pendingDocuments.Rows[i]["DocEndDate"].ToString();
                                pendingDocumentsRevisedRow["StatusGroup"] = status;

                                pendingDocumentsRevised.Rows.Add(pendingDocumentsRevisedRow);
                                pendingDocumentsRevisedRowCount = pendingDocumentsRevisedRowCount + 1;
                            }

                            categoryInfo = pendingDocuments.Rows[i]["CategoryInfo"].ToString();
                        }
                    }

                    List<DataRow> rowsToRemove = new List<DataRow>();

                    foreach (DataRow pendingDocumentsRevisedRow in pendingDocumentsRevised.Rows)
                    {
                        //"C" determines when it is CAN, REC or CAP
                        if (pendingDocumentsRevisedRow["StatusGroup"].ToString().Trim().ToUpper().Contains("C"))
                        {
                            rowsToRemove.Add(pendingDocumentsRevisedRow);
                        }
                    }

                    foreach (var dr in rowsToRemove)
                    {
                        pendingDocumentsRevised.Rows.Remove(dr);
                    }

                    return pendingDocumentsRevised;
                }
            }
        }

        #endregion

    }
}
