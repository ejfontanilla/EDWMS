<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FilteringUserControl.ascx.cs" Inherits="Control_FilteringUserControl" %>
 <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">

        <script type="text/javascript">


            function filterMenuShowing(sender, eventArgs) {

                column = eventArgs.get_column();

            }

            var column = null;
            function MenuShowing(sender, args) {

                if (column == null)
                    return;
                var menu = sender;

                var items = menu.get_items();
                if (column.get_dataType() == "System.String" || column.get_dataType() == "System.Decimal" ||
                column.get_dataType() == "System.Double") {

                    var i = 0;
                    while (i < items.get_count()) {
                        if (!(items.getItem(i).get_value() in
                        { 'NoFilter': '', 'EqualTo': '', 'NotEqualTo': '', 'StartsWith': '', 'EndsWith': '',
                            'Contains': '', 'DoesNotContain': ''
                        })) {
                            var item = items.getItem(i);
                            if (item != null)
                                item.set_visible(false);
                        }
                        else {
                            var item = items.getItem(i);
                            if (item != null)
                                item.set_visible(true);
                            if (items.getItem(i).get_value() == 'EqualTo') {

                                items.getItem(i).set_text('Equals');
                            }
                            else if (items.getItem(i).get_value() == 'NotEqualTo') {

                                items.getItem(i).set_text('Does Not Equal');
                            }
                            else if (items.getItem(i).get_value() == 'StartsWith') {

                                items.getItem(i).set_text('Begins With');
                            }
                            else if (items.getItem(i).get_value() == 'EndsWith') {

                                items.getItem(i).set_text('Ends With');
                            }
                            else if (items.getItem(i).get_value() == 'Contains') {

                                items.getItem(i).set_text('Contains');
                            }
                            else if (items.getItem(i).get_value() == 'DoesNotContain') {

                                items.getItem(i).set_text('Does Not Contain');
                            }
                            else if (items.getItem(i).get_value() == 'NoFilter') {

                                items.getItem(i).set_text('Blank');
                            }
                        }
                        i++;
                    }

                }
                else {

                    var i = 0;
                    while (i < items.get_count()) {
                        if (!(items.getItem(i).get_value() in { 'NoFilter': '', 'EqualTo': '', 'GreaterThan': '',
                            'LessThan': ''
                        })) {
                            var item = items.getItem(i);
                            if (item != null)
                                item.set_visible(false);
                        }
                        else {
                            var item = items.getItem(i);
                            if (item != null) {
                                item.set_visible(true);
                                if (items.getItem(i).get_value() == 'EqualTo') {

                                    items.getItem(i).set_text('Equals');
                                }
                                else if (items.getItem(i).get_value() == 'GreaterThan') {

                                    items.getItem(i).set_text('After');
                                }
                                else if (items.getItem(i).get_value() == 'LessThan') {

                                    items.getItem(i).set_text('Before');
                                }
                                else if (items.getItem(i).get_value() == 'NoFilter') {

                                    items.getItem(i).set_text('Blank');
                                }
                            }
                        }
                        i++;
                    }
                }
            }

        </script>

    </telerik:RadCodeBlock>
