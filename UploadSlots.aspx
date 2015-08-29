<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UploadSlots.aspx.cs" Inherits="WebCal.UploadSlots" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Upload Slots</title>

    <script language="javascript" type="text/javascript">
// <!CDATA[

// ]]>
    </script>
</head>
<body style="background: AntiqueWhite">
    <form id="form1" method="post" enctype="multipart/form-data" runat="server">
    <div align=right>
        <INPUT type=file id=File1 name=File1 runat="server" stype="background: AnqtiqueWhite" />
        <br><br>
        <input type="submit" id="Submit1" value="Upload" runat="server"  />            
    
    </div>
    </form>
</body>
</html>
