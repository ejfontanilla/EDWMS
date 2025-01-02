using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Web.UI;
using Telerik.Web.UI;



    public partial class IncomeAssessment_Test : System.Web.UI.Page
    {
        protected void UseDragColumnCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            grdPendingOrders.MasterTableView.GetColumn("DragDropColumn").Visible = UseDragColumnCheckBox.Checked;
            grdPendingOrders.Rebind();

            grdShippedOrders.MasterTableView.GetColumn("DragDropColumn").Visible = UseDragColumnCheckBox.Checked;
            grdShippedOrders.Rebind();
        }

        protected IList<Order> PendingOrders
        {
            get
            {
                try
                {
                    object obj = Session["PendingOrders"];
                    if (obj == null)
                    {
                        obj = GetOrders();
                        if (obj != null)
                        {
                            Session["PendingOrders"] = obj;
                        }
                        else
                        {
                            obj = new List<Order>();
                        }
                    }
                    return (IList<Order>)obj;
                }
                catch
                {
                    Session["PendingOrders"] = null;
                }
                return new List<Order>();
            }
            set { Session["PendingOrders"] = value; }
        }

        protected IList<Order> ShippedOrders
        {
            get
            {
                try
                {
                    object obj = Session["ShippedOrders"];
                    if (obj == null)
                    {
                        Session["ShippedOrders"] = obj = new List<Order>();
                    }
                    return (IList<Order>)obj;
                }
                catch
                {
                    Session["ShippedOrders"] = null;
                }
                return new List<Order>();
            }
            set { Session["ShippedOrders"] = value; }
        }

        protected void grdPendingOrders_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            grdPendingOrders.DataSource = PendingOrders;
        }

        protected IList<Order> GetOrders()
        {
            IList<Order> results = new List<Order>();
            using (IDbConnection connection = DbProviderFactories.GetFactory("System.Data.SqlClient").CreateConnection())
            {
                connection.ConnectionString =
                    ConfigurationManager.ConnectionStrings["NorthwindConnectionString35"].ConnectionString;

                using (IDbCommand command = connection.CreateCommand())
                {
                    command.CommandText =
                        "SELECT Id AS OrderID, IncomeVersionId AS CustomerId, '1/1/2012' AS RequiredDate, Name AS CompanyName FROM Income INNER JOIN IncomeVersion ON Income.Id = IncomeVersion.IncomeId INNER JOIN IncomeDetails ON IncomeVersion.Id = IncomeDetails.IncomeVersionID INNER JOIN IncomeDoc ON Income.Id = IncomeDoc.IncomeId INNER JOIN AppDocRef ON IncomeDoc.AppDocRefId = AppDocRef.Id INNER JOIN AppPersonal ON AppDocRef.AppPersonalId = AppPersonal.Id INNER JOIN Doc ON AppDocRef.DocId = Doc.Id  INNER JOIN DocType ON Doc.DocTypeCode = Doctype.Code WHERE AppPersonal.DocAppId = 51301 AND LTRIM(RTRIM(AppPersonal.Nric)) = LTRIM(RTRIM('S8846994A')) AND AppDocRef.Id = 91577";
                        //"SELECT o.OrderID, o.CustomerID, o.RequiredDate, c.CompanyName FROM orders o INNER JOIN customers c on o.customerID = c.customerID";
                        
                    connection.Open();
                    try
                    {
                        IDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            int id = (int)reader.GetValue(reader.GetOrdinal("OrderID"));
                            string customerID = (!reader.IsDBNull(reader.GetOrdinal("CustomerID")))
                                                    ? (string)reader.GetValue(reader.GetOrdinal("CustomerID"))
                                                    : string.Empty;
                            DateTime requiredDate = (!reader.IsDBNull(reader.GetOrdinal("RequiredDate")))
                                                        ? (DateTime)reader.GetValue(reader.GetOrdinal("RequiredDate"))
                                                        : DateTime.MinValue;
                            string companyName = (!reader.IsDBNull(reader.GetOrdinal("CompanyName")))
                                                     ? Server.HtmlEncode((string)reader.GetValue(reader.GetOrdinal("CompanyName")))
                                                     : string.Empty;
                            results.Add(new Order(id, customerID, companyName, requiredDate.ToShortDateString()));
                        }
                    }
                    catch
                    {
                        results.Clear();
                    }
                }
            }
            return results;
        }

        protected void grdShippedOrders_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            grdShippedOrders.DataSource = ShippedOrders;
        }

        protected void grdPendingOrders_RowDrop(object sender, GridDragDropEventArgs e)
        {
            if (string.IsNullOrEmpty(e.HtmlElement))
            {
                if (e.DraggedItems[0].OwnerGridID == grdPendingOrders.ClientID)
                {
                    // items are drag from pending to shipped grid
                    if ((e.DestDataItem == null && ShippedOrders.Count == 0) ||
                        e.DestDataItem != null && e.DestDataItem.OwnerGridID == grdShippedOrders.ClientID)
                    {
                        IList<Order> shippedOrders = ShippedOrders;
                        IList<Order> pendingOrders = PendingOrders;
                        int destinationIndex = -1;
                        if (e.DestDataItem != null)
                        {
                            Order order = GetOrder(shippedOrders, (int)e.DestDataItem.GetDataKeyValue("OrderId"));
                            destinationIndex = (order != null) ? shippedOrders.IndexOf(order) : -1;
                        }


                        foreach (GridDataItem draggedItem in e.DraggedItems)
                        {
                            Order tmpOrder = GetOrder(pendingOrders, (int)draggedItem.GetDataKeyValue("OrderId"));

                            if (tmpOrder != null)
                            {
                                if (destinationIndex > -1)
                                {
                                    if (e.DropPosition == GridItemDropPosition.Below)
                                    {
                                        destinationIndex += 1;
                                    }
                                    shippedOrders.Insert(destinationIndex, tmpOrder);
                                }
                                else
                                {
                                    shippedOrders.Add(tmpOrder);
                                }

                                pendingOrders.Remove(tmpOrder);
                            }
                        }

                        ShippedOrders = shippedOrders;
                        PendingOrders = pendingOrders;
                        grdPendingOrders.Rebind();
                        grdShippedOrders.Rebind();
                    }
                    else if (e.DestDataItem != null && e.DestDataItem.OwnerGridID == grdPendingOrders.ClientID)
                    {
                        //reorder items in pending  grid
                        IList<Order> pendingOrders = PendingOrders;
                        Order order = GetOrder(pendingOrders, (int)e.DestDataItem.GetDataKeyValue("OrderId"));
                        int destinationIndex = pendingOrders.IndexOf(order);

                        if (e.DropPosition == GridItemDropPosition.Above && e.DestDataItem.ItemIndex > e.DraggedItems[0].ItemIndex)
                        {
                            destinationIndex -= 1;
                        }
                        if (e.DropPosition == GridItemDropPosition.Below && e.DestDataItem.ItemIndex < e.DraggedItems[0].ItemIndex)
                        {
                            destinationIndex += 1;
                        }

                        List<Order> ordersToMove = new List<Order>();
                        foreach (GridDataItem draggedItem in e.DraggedItems)
                        {
                            Order tmpOrder = GetOrder(pendingOrders, (int)draggedItem.GetDataKeyValue("OrderId"));
                            if (tmpOrder != null)
                                ordersToMove.Add(tmpOrder);
                        }

                        foreach (Order orderToMove in ordersToMove)
                        {
                            pendingOrders.Remove(orderToMove);
                            pendingOrders.Insert(destinationIndex, orderToMove);
                        }
                        PendingOrders = pendingOrders;
                        grdPendingOrders.Rebind();

                        int destinationItemIndex = destinationIndex - (grdPendingOrders.PageSize * grdPendingOrders.CurrentPageIndex);
                        e.DestinationTableView.Items[destinationItemIndex].Selected = true;
                    }
                }
            }
        }

        private static Order GetOrder(IEnumerable<Order> ordersToSearchIn, int orderId)
        {
            foreach (Order order in ordersToSearchIn)
            {
                if (order.OrderID == orderId)
                {
                    return order;
                }
            }
            return null;
        }

        protected void grdShippedOrders_RowDrop(object sender, GridDragDropEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.HtmlElement) && e.HtmlElement == "trashCan")
            {
                IList<Order> shippedOrders = ShippedOrders;
                bool deleted = false;
                foreach (GridDataItem draggedItem in e.DraggedItems)
                {
                    Order tmpOrder = GetOrder(shippedOrders, (int)draggedItem.GetDataKeyValue("OrderId"));

                    if (tmpOrder != null)
                    {
                        shippedOrders.Remove(tmpOrder);
                        deleted = true;
                    }
                }
                if (deleted)
                {
                    msg.Visible = true;
                }
                ShippedOrders = shippedOrders;
                grdShippedOrders.Rebind();
            }
        }

        #region Nested type: Order

        protected class Order
        {
            private string _companyName;
            private string _customerId;
            private int _orderId;
            private string _date;

            public Order(int orderId, string customerId, string companyName, string requiredDate)
            {
                _orderId = orderId;
                _customerId = customerId;
                _companyName = companyName;
                _date = requiredDate;
            }

            public int OrderID
            {
                get { return _orderId; }
            }

            public string CustomerID
            {
                get { return _customerId; }
            }

            public string Company
            {
                get { return _companyName; }
            }

            public string Date
            {
                get { return _date; }
            }
        }

        #endregion
    }
