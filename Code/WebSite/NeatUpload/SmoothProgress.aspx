<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN"
        "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Page language="c#" AutoEventWireup="false" Inherits="Brettle.Web.NeatUpload.ProgressPage" %>
<%@ Register TagPrefix="Upload" Namespace="Brettle.Web.NeatUpload" Assembly="Brettle.Web.NeatUpload" %>
<%--
NeatUpload - an HttpModule and User Controls for uploading large files
Copyright (C) 2005  Dean Brettle

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
--%>
<html>
	<head runat="server">
		<title>Upload Progress</title>
		<link rel="stylesheet" type="text/css" title="default" href="default.css" />		
		<style type="text/css">
		body, form, table, tr, td {
			margin: 0px;
			border: 0px none;
			padding: 0px;
		}

		html, body, form, #progressDisplayCenterer {
			width: 100%;
			height: 100%;
		}
		
		#progressDisplayCenterer {
			vertical-align: middle;
			margin: 0 auto;
		}
		
		#progressDisplay {
			vertical-align: middle;
			width: 100%;
		}
		
		#barTd {
			width: 100%;
		}
		
		#statusDiv {
			border-width: 1px;
			border-style: solid;
			padding: 0px;
			position: relative;
			width: 100%;
			text-align: center;
			z-index: 1; 
		}
		
		#barDiv,#barDetailsDiv,#smoothBar {
			border: 0px none ; 
			margin: 0px; 
			padding: 0px; 
			position: absolute; 
			top: 0pt; 
			left: 0pt; 
			z-index: -1; 
			height: 100%;
			width: 75%;
		}
		</style>
	</head>
	<body>
		<form id="dummyForm" runat="server">
		<table id="progressDisplayCenterer">
		<tr>
		<td>
		<table id="progressDisplay" class="ProgressDisplay">
		<tr>
		<td>
			<span id="label" runat="server" class="Label">Upload&#160;Status:</span>
		</td>
		<td id="barTd" >
			<div id="statusDiv" runat="server" class="StatusMessage">&#160;
				<Upload:DetailsSpan id="normalInProgress" runat="server" WhenStatus="NormalInProgress" style="font-weight: normal; white-space: nowrap;">
					<%# FormatCount(BytesRead) %>/<%# FormatCount(BytesTotal) %> <%# CountUnits %>
					(<%# String.Format("{0:0%}", FractionComplete) %>) at <%# FormatRate(BytesPerSec) %>
					- <%# FormatTimeSpan(TimeRemaining) %> left
				</Upload:DetailsSpan>
				<Upload:DetailsSpan id="chunkedInProgress" runat="server" WhenStatus="ChunkedInProgress" style="font-weight: normal; white-space: nowrap;">
					<%# FormatCount(BytesRead) %> <%# CountUnits %>
					at <%# FormatRate(BytesPerSec) %>
					- <%# FormatTimeSpan(TimeElapsed) %> elapsed
				</Upload:DetailsSpan>
				<Upload:DetailsSpan id="processing" runat="server" WhenStatus="ProcessingInProgress ProcessingCompleted" style="font-weight: normal; white-space: nowrap;">
					<%# ProcessingHtml %>
				</Upload:DetailsSpan>
				<Upload:DetailsSpan id="completed" runat="server" WhenStatus="Completed">
					Complete: <%# FormatCount(BytesRead) %> <%# CountUnits %>
					at <%# FormatRate(BytesPerSec) %>
					took <%# FormatTimeSpan(TimeElapsed) %>
				</Upload:DetailsSpan>
				<Upload:DetailsSpan id="cancelled" runat="server" WhenStatus="Cancelled">
					Cancelled!
				</Upload:DetailsSpan>
				<Upload:DetailsSpan id="rejected" runat="server" WhenStatus="Rejected">
					Rejected: <%# Rejection != null ? Rejection.Message : "" %>
				</Upload:DetailsSpan>
				<Upload:DetailsSpan id="error" runat="server" WhenStatus="Failed">
					Error: <%# Failure != null ? Failure.Message : "" %>
				</Upload:DetailsSpan>
				<Upload:DetailsDiv id="barDetailsDiv" runat="server" UseHtml4="true"
					 FractionComplete='<%# FractionComplete %>'
					 Status='<%# Status %>'
					 Width='<%# Unit.Percentage(Math.Floor(100*FractionComplete)) %>' CssClass="ProgressBar"></Upload:DetailsDiv>	
			</div>
		</td>
		<td>
			<asp:HyperLink id="cancel" runat="server" Visible='<%# CancelVisible %>' NavigateUrl='<%# CancelUrl %>' ToolTip="Cancel Upload" CssClass="ImageButton" ><img id="cancelImage" src="cancel.png" alt="Cancel Upload" /></asp:HyperLink>
			<asp:HyperLink id="refresh" runat="server" Visible='<%# StartRefreshVisible %>' NavigateUrl='<%# StartRefreshUrl %>' ToolTip="Refresh" CssClass="ImageButton" ><img id="refreshImage" src="refresh.png" alt="Refresh" /></asp:HyperLink>
			<asp:HyperLink id="stopRefresh" runat="server" Visible='<%# StopRefreshVisible %>' NavigateUrl='<%# StopRefreshUrl %>' ToolTip="Stop Refreshing" CssClass="ImageButton"><img id="stopRefreshImage" src="stop_refresh.png" alt="Stop Refreshing" /></asp:HyperLink>
		</td>
		</tr>
		</table>
		</td>
		</tr>
		</table>
		</form>
	</body>
<script type="text/javascript">
<!--
	var barDetailsDiv = document.getElementById('barDetailsDiv');
	var status = barDetailsDiv.getAttribute('Status');
	if (status == 'NormalInProgress')
	{
		// Fake a smoother progress bar by moving the bar 1/10 of the way
		// to the actual FractionComplete every 1/10 of a second.  The bar that
		// is displayed will be about 1 sec behind the real progress but will 
		// move more smoothly.
		var lastFractionComplete = barDetailsDiv.getAttribute('FractionComplete');
		var smoothFractionComplete = lastFractionComplete;
		var fractionCompleteDelta = 0;
		// Replace the bar with a new bar with a different id.
		var smoothBar = barDetailsDiv.cloneNode(true);
		smoothBar.id = "smoothBar";
		barDetailsDiv.parentNode.replaceChild(smoothBar, barDetailsDiv);
		// Make a new dummy element with the same id as the original bar so that
		// Progress.js can update the real FractionComplete.
		barDetailsDiv = document.createElement('dummy');
		barDetailsDiv.id = 'barDetailsDiv';
		document.body.insertBefore(barDetailsDiv, null);
		// Initialize its FractionComplete attribute.
		barDetailsDiv.setAttribute('FractionComplete', lastFractionComplete);
		// Every 1/10 of a second, check whether Progress.js has changed the
		// FractionComplete element, and update our smooth bar.
		window.setInterval(function () {
			var fractionComplete = barDetailsDiv.getAttribute('FractionComplete');
			if (fractionComplete != lastFractionComplete)
			{
				fractionCompleteDelta = (fractionComplete - lastFractionComplete)/10.0;
				lastFractionComplete = fractionComplete;
			}
			smoothFractionComplete = Math.min(fractionComplete, smoothFractionComplete + fractionCompleteDelta);
			smoothBar.style.width = Math.floor(smoothFractionComplete * 100) + "%";
		}, 100);
	}
// -->
</script>	 
</html>
