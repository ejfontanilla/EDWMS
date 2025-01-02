using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Dwms.Bll;
using Dwms.Web;
using System.Collections.Generic;

public partial class IncomeAssessment_AssignmentHle : System.Web.UI.Page
{
    //string user;              Commented by Edward 2016/02/04 take out unused variables
    //int sectionId = -1;       Commented by Edward 2016/02/04 take out unused variables

    #region Event Handlers
    protected void Page_Load(object sender, EventArgs e)
    {
        ConfirmPanel.Visible = (Request["cfm"] == "1");

        if (!IsPostBack)
        {
            PopulateRepeater();
        }
    }

    protected void PendingAssignmentRadGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
    {
        DocAppDb docAppDb = new DocAppDb();
        PendingAssignmentRadGrid.DataSource = docAppDb.GetPendingHleCountsIncomeAssessment(DateTime.Now, AssessmentStatusEnum.Completeness_Checked.ToString());
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        foreach (RepeaterItem rItem in Repeater1.Items)
        {
            TextBox ATextBox = rItem.FindControl("ATextBox") as TextBox;
            TextBox BTextBox = rItem.FindControl("BTextBox") as TextBox;
            TextBox CTextBox = rItem.FindControl("CTextBox") as TextBox;
            TextBox DTextBox = rItem.FindControl("DTextBox") as TextBox;
            TextBox ETextBox = rItem.FindControl("ETextBox") as TextBox;
            TextBox FTextBox = rItem.FindControl("FTextBox") as TextBox;
            TextBox HTextBox = rItem.FindControl("HTextBox") as TextBox;
            TextBox LTextBox = rItem.FindControl("LTextBox") as TextBox;
            TextBox NTextBox = rItem.FindControl("NTextBox") as TextBox;
            TextBox TTextBox = rItem.FindControl("TTextBox") as TextBox;
            TextBox XTextBox = rItem.FindControl("XTextBox") as TextBox;

            if (ATextBox != null)
            {
                ATextBox.Attributes.Add("onchange", String.Format("javascript:Validate('{0}');", ATextBox.ClientID));
            }

            if (BTextBox != null)
            {
                BTextBox.Attributes.Add("onchange", String.Format("javascript:Validate('{0}');", BTextBox.ClientID));
            }

            if (CTextBox != null)
            {
                CTextBox.Attributes.Add("onchange", String.Format("javascript:Validate('{0}');", CTextBox.ClientID));
            }

            if (DTextBox != null)
            {
                DTextBox.Attributes.Add("onchange", String.Format("javascript:Validate('{0}');", DTextBox.ClientID));
            }

            if (ETextBox != null)
            {
                ETextBox.Attributes.Add("onchange", String.Format("javascript:Validate('{0}');", ETextBox.ClientID));
            }

            if (FTextBox != null)
            {
                FTextBox.Attributes.Add("onchange", String.Format("javascript:Validate('{0}');", FTextBox.ClientID));
            }

            if (HTextBox != null)
            {
                HTextBox.Attributes.Add("onchange", String.Format("javascript:Validate('{0}');", HTextBox.ClientID));
            }

            if (LTextBox != null)
            {
                LTextBox.Attributes.Add("onchange", String.Format("javascript:Validate('{0}');", LTextBox.ClientID));
            }

            if (NTextBox != null)
            {
                NTextBox.Attributes.Add("onchange", String.Format("javascript:Validate('{0}');", NTextBox.ClientID));
            }

            if (TTextBox != null)
            {
                TTextBox.Attributes.Add("onchange", String.Format("javascript:Validate('{0}');", TTextBox.ClientID));
            }

            if (XTextBox != null)
            {
                XTextBox.Attributes.Add("onchange", String.Format("javascript:Validate('{0}');", XTextBox.ClientID));
            }
        }
    }

    protected void Repeater1_ItemCreated(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Footer)
        {
            Label FooterATotalLabel = e.Item.FindControl("FooterATotalLabel") as Label;
            Label FooterBTotalLabel = e.Item.FindControl("FooterBTotalLabel") as Label;
            Label FooterCTotalLabel = e.Item.FindControl("FooterCTotalLabel") as Label;
            Label FooterDTotalLabel = e.Item.FindControl("FooterDTotalLabel") as Label;
            Label FooterETotalLabel = e.Item.FindControl("FooterETotalLabel") as Label;
            Label FooterFTotalLabel = e.Item.FindControl("FooterFTotalLabel") as Label;
            Label FooterHTotalLabel = e.Item.FindControl("FooterHTotalLabel") as Label;
            Label FooterLTotalLabel = e.Item.FindControl("FooterLTotalLabel") as Label;
            Label FooterNTotalLabel = e.Item.FindControl("FooterNTotalLabel") as Label;
            Label FooterTTotalLabel = e.Item.FindControl("FooterTTotalLabel") as Label;
            Label FooterXTotalLabel = e.Item.FindControl("FooterXTotalLabel") as Label;
            Label FooterTotalLabel = e.Item.FindControl("FooterTotalLabel") as Label;

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "AssignAFooterTotalLabelScript",
                String.Format("javascript:SetFooterTotalLabelsId('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}');",
                FooterATotalLabel.ClientID, FooterBTotalLabel.ClientID, FooterCTotalLabel.ClientID, FooterDTotalLabel.ClientID, FooterETotalLabel.ClientID, FooterFTotalLabel.ClientID,
                FooterHTotalLabel.ClientID, FooterLTotalLabel.ClientID, FooterNTotalLabel.ClientID, FooterTTotalLabel.ClientID, FooterXTotalLabel.ClientID,
                FooterTotalLabel.ClientID), true);
        }
        else if (e.Item.ItemType == ListItemType.Header)
        {
            Label HeaderATotalLabel = e.Item.FindControl("HeaderATotalLabel") as Label;
            Label HeaderBTotalLabel = e.Item.FindControl("HeaderBTotalLabel") as Label;
            Label HeaderCTotalLabel = e.Item.FindControl("HeaderCTotalLabel") as Label;
            Label HeaderDTotalLabel = e.Item.FindControl("HeaderDTotalLabel") as Label;
            Label HeaderETotalLabel = e.Item.FindControl("HeaderETotalLabel") as Label;
            Label HeaderFTotalLabel = e.Item.FindControl("HeaderFTotalLabel") as Label;
            Label HeaderHTotalLabel = e.Item.FindControl("HeaderHTotalLabel") as Label;
            Label HeaderLTotalLabel = e.Item.FindControl("HeaderLTotalLabel") as Label;
            Label HeaderNTotalLabel = e.Item.FindControl("HeaderNTotalLabel") as Label;
            Label HeaderTTotalLabel = e.Item.FindControl("HeaderTTotalLabel") as Label;
            Label HeaderXTotalLabel = e.Item.FindControl("HeaderXTotalLabel") as Label;
            Label HeaderTotalLabel = e.Item.FindControl("HeaderTotalLabel") as Label;

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "AssignAHeaderTotalLabelScript",
                String.Format("javascript:SetHeaderTotalLabelsId('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}');",
                HeaderATotalLabel.ClientID, HeaderBTotalLabel.ClientID, HeaderCTotalLabel.ClientID, HeaderDTotalLabel.ClientID, HeaderETotalLabel.ClientID, HeaderFTotalLabel.ClientID,
                HeaderHTotalLabel.ClientID, HeaderLTotalLabel.ClientID, HeaderNTotalLabel.ClientID, HeaderTTotalLabel.ClientID, HeaderXTotalLabel.ClientID,
                HeaderTotalLabel.ClientID), true);
        }
    }

    protected void AssignButton_Click(object sender, EventArgs e)
    {
        foreach (RepeaterItem item in Repeater1.Items)
        {
            // Update DocApps Here
            Guid userId = new Guid((item.FindControl("UserHiddenField") as HiddenField).Value); 
            int laneA = int.Parse((item.FindControl("ATextBox") as TextBox).Text);
            int laneB = int.Parse((item.FindControl("BTextBox") as TextBox).Text);
            int laneC = int.Parse((item.FindControl("CTextBox") as TextBox).Text);
            int laneD = int.Parse((item.FindControl("DTextBox") as TextBox).Text);
            int laneE = int.Parse((item.FindControl("ETextBox") as TextBox).Text);
            int laneF = int.Parse((item.FindControl("FTextBox") as TextBox).Text);
            int laneH = int.Parse((item.FindControl("HTextBox") as TextBox).Text);
            int laneL = int.Parse((item.FindControl("LTextBox") as TextBox).Text);
            int laneN = int.Parse((item.FindControl("NTextBox") as TextBox).Text);
            int laneT = int.Parse((item.FindControl("TTextBox") as TextBox).Text);
            int laneX = int.Parse((item.FindControl("XTextBox") as TextBox).Text);

            DocAppDb docAppDb = new DocAppDb();

            if (laneA > 0.0)
            {
                docAppDb.UpdateTopPendingHleByLanesIA(userId, laneA, HleLanesEnum.A);
            }

            if (laneB > 0.0)
            {
                docAppDb.UpdateTopPendingHleByLanesIA(userId, laneB, HleLanesEnum.B);
            }

            if (laneC > 0.0)
            {
                docAppDb.UpdateTopPendingHleByLanesIA(userId, laneC, HleLanesEnum.C);
            }

            if (laneD > 0.0)
            {
                docAppDb.UpdateTopPendingHleByLanesIA(userId, laneD, HleLanesEnum.D);
            }

            if (laneE > 0.0)
            {
                docAppDb.UpdateTopPendingHleByLanesIA(userId, laneE, HleLanesEnum.E);
            }

            if (laneF > 0.0)
            {
                docAppDb.UpdateTopPendingHleByLanesIA(userId, laneF, HleLanesEnum.F);
            }

            if (laneH > 0.0)
            {
                docAppDb.UpdateTopPendingHleByLanesIA(userId, laneH, HleLanesEnum.H);
            }

            if (laneL > 0.0)
            {
                docAppDb.UpdateTopPendingHleByLanesIA(userId, laneL, HleLanesEnum.L);
            }

            if (laneN > 0.0)
            {
                docAppDb.UpdateTopPendingHleByLanesIA(userId, laneN, HleLanesEnum.N);
            }

            if (laneT > 0.0)
            {
                docAppDb.UpdateTopPendingHleByLanesIA(userId, laneT, HleLanesEnum.T);
            }

            if (laneX > 0.0)
            {
                docAppDb.UpdateTopPendingHleByLanesIA(userId, laneX, HleLanesEnum.X);
            }
        }

        Response.Redirect("AssignmentHle.aspx?cfm=1");
    }

    protected void Repeater1_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item && e.Item.ItemType == ListItemType.AlternatingItem)
        {
            //TextBox ATextBox = e.Item.FindControl("ATextBox") as TextBox;
            //ATextBox.Attributes.Add("onclick", String.Format("javascript:Validate('{0}');", ATextBox.ClientID));
        }
    }
    #endregion

    #region Private Methods
    private void PopulateRepeater()
    {
        DocAppDb docAppDb = new DocAppDb();
        Dictionary<string, double> laneTotals = new Dictionary<string, double>();

        Repeater1.DataSource = docAppDb.GetAssessmentOfficerForCaseAssignment(ref laneTotals);
        Repeater1.DataBind();

        Dictionary<string, int> pendingHleCounts = docAppDb.GetPendingHleCountsByAllLanesIA();

        LaneAMaxHiddenField.Value = LaneAUnassignedHiddenField.Value = pendingHleCounts[HleLanesEnum.A.ToString()].ToString();
        LaneBMaxHiddenField.Value = LaneBUnassignedHiddenField.Value = pendingHleCounts[HleLanesEnum.B.ToString()].ToString();
        LaneCMaxHiddenField.Value = LaneCUnassignedHiddenField.Value = pendingHleCounts[HleLanesEnum.C.ToString()].ToString();
        LaneDMaxHiddenField.Value = LaneDUnassignedHiddenField.Value = pendingHleCounts[HleLanesEnum.D.ToString()].ToString();
        LaneEMaxHiddenField.Value = LaneEUnassignedHiddenField.Value = pendingHleCounts[HleLanesEnum.E.ToString()].ToString();
        LaneFMaxHiddenField.Value = LaneFUnassignedHiddenField.Value = pendingHleCounts[HleLanesEnum.F.ToString()].ToString();
        LaneHMaxHiddenField.Value = LaneHUnassignedHiddenField.Value = pendingHleCounts[HleLanesEnum.H.ToString()].ToString();
        LaneLMaxHiddenField.Value = LaneLUnassignedHiddenField.Value = pendingHleCounts[HleLanesEnum.L.ToString()].ToString();
        LaneNMaxHiddenField.Value = LaneNUnassignedHiddenField.Value = pendingHleCounts[HleLanesEnum.N.ToString()].ToString();
        LaneTMaxHiddenField.Value = LaneTUnassignedHiddenField.Value = pendingHleCounts[HleLanesEnum.T.ToString()].ToString();
        LaneXMaxHiddenField.Value = LaneXUnassignedHiddenField.Value = pendingHleCounts[HleLanesEnum.X.ToString()].ToString();
    }
    #endregion
}
