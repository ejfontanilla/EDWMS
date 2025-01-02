using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dwms.Bll;
using CdbWebRef;

public partial class Test_WebServices_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        // Before we can use Cdb2WebRef, a web reference must be added under the App_WebReferences folder of the ASP.NET website.
        //Cdb webService = new Cdb();
        CdbWebRef.Cdb webService = new CdbWebRef.Cdb();

        // Set authentication header. This header optional for now and will be turned on in the future to enhance security
        AuthenticationClass authentication = new AuthenticationClass();
        authentication.UserName = "ABCDEinterface";
        authentication.Password = "abc123";

        AcknowledgementInfoClass ackInfo = new AcknowledgementInfoClass();
        ackInfo.AcknowledgementNo = "DEF011";
        ackInfo.AcknowledgementDate = DateTime.Now;
        ackInfo.ApplicantIdentityNo = string.Empty;

        // ##################### CustomerClass #####################
        CustomerClass[] customers = new CustomerClass[1];

        customers[0] = new CustomerClass();
        customers[0].IdentityNo = "S1114155B";
        customers[0].IdentityType = "UIN";
        customers[0].CustomerIdFromSource = "ABCDEFGHI";
        customers[0].CustomerType = "P";
        customers[0].DocCounter = 1;

        DocumentClass[] documents = new DocumentClass[1];
        documents[0] = new DocumentClass();
        documents[0].DocId = "000026"; //000036
        documents[0].DocIdSub = "00";
        documents[0].DocDescription = string.Empty;
        documents[0].DocStartDate = new DateTime(2012, 11, 1);
        documents[0].DocEndDate = new DateTime(2013, 10, 1);
        documents[0].IdentityNoSub = string.Empty;
        documents[0].CustomerIdSubFromSource = string.Empty;
        documents[0].RequesterNickname = string.Empty;

        ImageInfoClass[] imageInfoSet1 = new ImageInfoClass[1];

        imageInfoSet1[0] = new ImageInfoClass();
        imageInfoSet1[0].ImageSize = 2575.1259765625m;
        imageInfoSet1[0].ImageName = "IMG_0940.JPG";
        imageInfoSet1[0].ImageUrl = @"c:\projects";
        imageInfoSet1[0].DateReceivedFromSource = DateTime.Now;
        imageInfoSet1[0].CmDocumentId = "A1001001A13J17C15512A04825";
        imageInfoSet1[0].IsAccepted = false;
        imageInfoSet1[0].IsMatchedWithExternalOrg = false;
        imageInfoSet1[0].DateFiled = DateTime.Now;
        imageInfoSet1[0].CertificateNumber = string.Empty;
        imageInfoSet1[0].CertificateDate = DateTime.Now;
        imageInfoSet1[0].LocalForeign = string.Empty;
        imageInfoSet1[0].MarriageType = string.Empty;
        imageInfoSet1[0].IsVerified = false;
        imageInfoSet1[0].DocChannel = "011";

        BusinessInfoClass[] businessInfoSet1 = new BusinessInfoClass[3];
        businessInfoSet1[0] = new BusinessInfoClass();
        businessInfoSet1[0].BusinessTransactionNumber = "HLE";
        businessInfoSet1[0].BusinessRefNumber = "R13X00374"; //N12N22594
        businessInfoSet1[1] = new BusinessInfoClass();
        businessInfoSet1[1].BusinessTransactionNumber = "HLE";
        businessInfoSet1[1].BusinessRefNumber = "R13X00374"; //N12N22594
        businessInfoSet1[2] = new BusinessInfoClass();
        businessInfoSet1[2].BusinessTransactionNumber = "HLE";
        businessInfoSet1[2].BusinessRefNumber = "R13X00374"; //N12N22594

        documents[0].ImageInfo = imageInfoSet1;
        documents[0].BusinessInfoSet = businessInfoSet1;



        //documents[1] = new DocumentClass();
        //documents[1].DocId = "000030"; //000036
        //documents[1].DocIdSub = "00";
        //documents[1].DocDescription = string.Empty;
        //documents[1].DocStartDate = DateTime.Now;
        //documents[1].DocEndDate = DateTime.Now;
        //documents[1].IdentityNoSub = string.Empty;
        //documents[1].CustomerIdSubFromSource = string.Empty;
        //documents[1].RequesterNickname = string.Empty;

        //ImageInfoClass[] imageInfoSet2 = new ImageInfoClass[1];
        //imageInfoSet2[0] = new ImageInfoClass();
        //imageInfoSet2[0].ImageSize = 1861.1796875m;
        //imageInfoSet2[0].ImageName = "IMG_0998.JPG";
        //imageInfoSet2[0].ImageUrl = @"c:\projects";
        //imageInfoSet2[0].DateReceivedFromSource = DateTime.Now;
        //imageInfoSet2[0].CmDocumentId = "A1001001A13J17C15512C85110";
        //imageInfoSet2[0].IsAccepted = false;
        //imageInfoSet2[0].IsMatchedWithExternalOrg = false;
        //imageInfoSet2[0].DateFiled = DateTime.Now;
        //imageInfoSet2[0].CertificateNumber = string.Empty;
        //imageInfoSet2[0].CertificateDate = DateTime.Now;
        //imageInfoSet2[0].LocalForeign = string.Empty;
        //imageInfoSet2[0].MarriageType = string.Empty;
        //imageInfoSet2[0].IsVerified = false;
        //imageInfoSet2[0].DocChannel = "002";

        //BusinessInfoClass[] businessInfoSet2 = new BusinessInfoClass[4];
        //businessInfoSet2[0] = new BusinessInfoClass();
        //businessInfoSet2[0].BusinessTransactionNumber = "HLE";
        //businessInfoSet2[0].BusinessRefNumber = "N14D90043"; //N12N22594
        //businessInfoSet2[1] = new BusinessInfoClass();
        //businessInfoSet2[1].BusinessTransactionNumber = "HLE";
        //businessInfoSet2[1].BusinessRefNumber = "N14D90043"; //N12N22594
        //businessInfoSet2[2] = new BusinessInfoClass();
        //businessInfoSet2[2].BusinessTransactionNumber = "HLE";
        //businessInfoSet2[2].BusinessRefNumber = "N14D90043"; //N12N22594
        //businessInfoSet2[3] = new BusinessInfoClass();
        //businessInfoSet2[3].BusinessTransactionNumber = "HLE";
        //businessInfoSet2[3].BusinessRefNumber = "N14D90043"; //N12N22594

        //documents[1].ImageInfo = imageInfoSet2;
        //documents[1].BusinessInfoSet = businessInfoSet2;


        //documents[2] = new DocumentClass();
        //documents[2].DocId = "000031"; //000036
        //documents[2].DocIdSub = "00";
        //documents[2].DocDescription = string.Empty;
        //documents[2].DocStartDate = DateTime.Now;
        //documents[2].DocEndDate = DateTime.Now;
        //documents[2].IdentityNoSub = string.Empty;
        //documents[2].CustomerIdSubFromSource = string.Empty;
        //documents[2].RequesterNickname = string.Empty;

        //ImageInfoClass[] imageInfoSet3 = new ImageInfoClass[1];
        //imageInfoSet3[0] = new ImageInfoClass();
        //imageInfoSet3[0].ImageSize = 2235.869140625m;
        //imageInfoSet3[0].ImageName = "IMG_1000.JPG";
        //imageInfoSet3[0].ImageUrl = @"c:\projects";
        //imageInfoSet3[0].DateReceivedFromSource = DateTime.Now;
        //imageInfoSet3[0].CmDocumentId = "A1001001A13J17C15512E97335";
        //imageInfoSet3[0].IsAccepted = false;
        //imageInfoSet3[0].IsMatchedWithExternalOrg = false;
        //imageInfoSet3[0].DateFiled = DateTime.Now;
        //imageInfoSet3[0].CertificateNumber = string.Empty;
        //imageInfoSet3[0].CertificateDate = DateTime.Now;
        //imageInfoSet3[0].LocalForeign = string.Empty;
        //imageInfoSet3[0].MarriageType = string.Empty;
        //imageInfoSet3[0].IsVerified = false;
        //imageInfoSet3[0].DocChannel = "002";

        //BusinessInfoClass[] businessInfoSet3 = new BusinessInfoClass[3];
        //businessInfoSet3[0] = new BusinessInfoClass();
        //businessInfoSet3[0].BusinessTransactionNumber = "HLE";
        //businessInfoSet3[0].BusinessRefNumber = "N14D90043"; //N12N22594
        //businessInfoSet3[1] = new BusinessInfoClass();
        //businessInfoSet3[1].BusinessTransactionNumber = "HLE";
        //businessInfoSet3[1].BusinessRefNumber = "N14D90043"; //N12N22594
        //businessInfoSet3[2] = new BusinessInfoClass();
        //businessInfoSet3[2].BusinessTransactionNumber = "HLE";
        //businessInfoSet3[2].BusinessRefNumber = "N14D90043"; //N12N22594

        //documents[2].ImageInfo = imageInfoSet3;
        //documents[2].BusinessInfoSet = businessInfoSet3;


        //documents[3] = new DocumentClass();
        //documents[3].DocId = "000034"; //000036
        //documents[3].DocIdSub = "00";
        //documents[3].DocDescription = string.Empty;
        //documents[3].DocStartDate = DateTime.Now;
        //documents[3].DocEndDate = DateTime.Now;
        //documents[3].IdentityNoSub = string.Empty;
        //documents[3].CustomerIdSubFromSource = string.Empty;
        //documents[3].RequesterNickname = string.Empty;

        //ImageInfoClass[] imageInfoSet4 = new ImageInfoClass[1];
        //imageInfoSet4[0] = new ImageInfoClass();
        //imageInfoSet4[0].ImageSize = 2454.1005859375m;
        //imageInfoSet4[0].ImageName = "IMG_1004.JPG";
        //imageInfoSet4[0].ImageUrl = @"c:\projects";
        //imageInfoSet4[0].DateReceivedFromSource = DateTime.Now;
        //imageInfoSet4[0].CmDocumentId = "A1001001A13J17C15512H02401";
        //imageInfoSet4[0].IsAccepted = false;
        //imageInfoSet4[0].IsMatchedWithExternalOrg = false;
        //imageInfoSet4[0].DateFiled = DateTime.Now;
        //imageInfoSet4[0].CertificateNumber = string.Empty;
        //imageInfoSet4[0].CertificateDate = DateTime.Now;
        //imageInfoSet4[0].LocalForeign = string.Empty;
        //imageInfoSet4[0].MarriageType = string.Empty;
        //imageInfoSet4[0].IsVerified = false;
        //imageInfoSet4[0].DocChannel = "002";



        //BusinessInfoClass[] businessInfoSet4 = new BusinessInfoClass[4];
        //businessInfoSet4[0] = new BusinessInfoClass();
        //businessInfoSet4[0].BusinessTransactionNumber = "HLE";
        //businessInfoSet4[0].BusinessRefNumber = "N14D90043"; //N12N22594
        //businessInfoSet4[1] = new BusinessInfoClass();
        //businessInfoSet4[1].BusinessTransactionNumber = "HLE";
        //businessInfoSet4[1].BusinessRefNumber = "N14D90043"; //N12N22594
        //businessInfoSet4[2] = new BusinessInfoClass();
        //businessInfoSet4[2].BusinessTransactionNumber = "HLE";
        //businessInfoSet4[2].BusinessRefNumber = "N14D90043"; //N12N22594
        //businessInfoSet4[3] = new BusinessInfoClass();
        //businessInfoSet4[3].BusinessTransactionNumber = "HLE";
        //businessInfoSet4[3].BusinessRefNumber = "N14D90043"; //N12N22594     

        //documents[3].ImageInfo = imageInfoSet4;
        //documents[3].BusinessInfoSet = businessInfoSet4;

        customers[0].Documents = documents;
        //customers[1].Documents = documents2;
        Label1.Text = DateTime.Now.ToString() + Environment.NewLine;
        int returnValue= webService.SendDocumentInfo(authentication, ackInfo, customers);
        Label1.Text = Label1.Text +  DateTime.Now.ToString() + Environment.NewLine;
        Label1.Text = Label1.Text +  returnValue.ToString();
    }
}



//PersonIdentityClass[] personIdentityInfoSet = new PersonIdentityClass[1];
////person
//personIdentityInfoSet[0] = new PersonIdentityClass();
//personIdentityInfoSet[0].customerIdFromSource = "customerIdFromSource";
//personIdentityInfoSet[0].customerName = "customerName";
//personIdentityInfoSet[0].identityNo = "identityNo";
//personIdentityInfoSet[0].identityType = "identityType";

//PersonInfoClass[] personInfoSet = new PersonInfoClass[1];
//personInfoSet[0] = new PersonInfoClass();
//personInfoSet[0].customerName = "customerName";
//personInfoSet[0].identityNo = "identityNo";
//personInfoSet[0].identityType = "identityType";

//personIdentityInfoSet[0].PersonInfoSet = personInfoSet;
//imageInfoSet[0].PersonIdentityInfoSet = personIdentityInfoSet;

//imageInfoSet[1] = new ImageInfoClass();
//imageInfoSet[1].ImageSize = 129.5302734375m;
//imageInfoSet[1].ImageName = "sample.jpg";
//imageInfoSet[1].ImageUrl = "http://heams2/DWMS/Test/Li Fuwei passport.jpg";
//imageInfoSet[1].DateReceivedFromSource = DateTime.Now;

//imageInfoSet[1].CmDocumentId = "CmDocumentId";
//imageInfoSet[1].IsAccepted = false;
//imageInfoSet[1].IsMatchedWithExternalOrg = false;
//imageInfoSet[1].DateFiled = DateTime.Now;
//imageInfoSet[1].DocChannel = "001";

//imageInfoSet[1].CertificateNumber = "CertificateNumber";
//imageInfoSet[1].CertificateDate = DateTime.Now;
//imageInfoSet[1].LocalForeign = "L";
//imageInfoSet[1].MarriageType = "c";
//imageInfoSet[1].IsVerified = false;