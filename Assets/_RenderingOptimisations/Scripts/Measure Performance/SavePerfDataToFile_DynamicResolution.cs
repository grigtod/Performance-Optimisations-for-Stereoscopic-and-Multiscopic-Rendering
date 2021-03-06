﻿using UnityEngine;
using System.Collections;
using System.IO;

public class SavePerfDataToFile_DynamicResolution : SavePerfDataToFile_Base 
{
    public DynamicResolutionManager dynamicResolutionManagerRef;
	public override void RecordNew ()
	{
		RecordDataStruct newRec;
		newRec.fps = (int)perfManager.fps;
		newRec.ms = perfManager.ms;
        newRec.float1 = dynamicResolutionManagerRef.targetFPS;
        newRec.float2 = dynamicResolutionManagerRef.resolutionMultiplierWidth;
        newRec.float3 = dynamicResolutionManagerRef.resolutionMultiplierHeight;
        newRec.float4 = Screen.width * dynamicResolutionManagerRef.resolutionMultiplierWidth;
        newRec.float5 = Screen.height * dynamicResolutionManagerRef.resolutionMultiplierHeight;
        newRec.method = (int)dynamicResolutionManagerRef.dynamicResolutionMode;
        recordedData.Add(newRec);   
	}

	public override void RecordDataToFile()
	{
		var sr = File.CreateText(fileName+"_"+System.DateTime.Now.TimeOfDay.Hours+System.DateTime.Now.TimeOfDay.Minutes +"_"+ System.DateTime.Now.Year+"."+System.DateTime.Now.Month+"."+System.DateTime.Now.Day+".xml");
		
		sr.WriteLine ("<?xml version=\"1.0\"?>");
		sr.WriteLine ("<?mso-application progid=\"Excel.Sheet\"?>");
		sr.WriteLine ("<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"");
		sr.WriteLine ("xmlns:o=\"urn:schemas-microsoft-com:office:office\"");
		sr.WriteLine ("xmlns:x=\"urn:schemas-microsoft-com:office:excel\"");
		sr.WriteLine ("xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\"");
		sr.WriteLine ("xmlns:html=\"http://www.w3.org/TR/REC-html40\">");
		sr.WriteLine ("<DocumentProperties xmlns=\"urn:schemas-microsoft-com:office:office\">");
		sr.WriteLine ("<Created>2006-09-16T00:00:00Z</Created>");
		sr.WriteLine ("<LastSaved>2015-08-09T17:55:48Z</LastSaved>");
		sr.WriteLine ("<Version>15.00</Version>");
		sr.WriteLine ("</DocumentProperties>");
		sr.WriteLine ("<OfficeDocumentSettings xmlns=\"urn:schemas-microsoft-com:office:office\">");
		sr.WriteLine ("<AllowPNG/>");
		sr.WriteLine ("<RemovePersonalInformation/>");
		sr.WriteLine ("</OfficeDocumentSettings>");
		sr.WriteLine ("<ExcelWorkbook xmlns=\"urn:schemas-microsoft-com:office:excel\">");
		sr.WriteLine ("<WindowHeight>7530</WindowHeight>");
		sr.WriteLine ("<WindowWidth>14370</WindowWidth>");
		sr.WriteLine ("<WindowTopX>0</WindowTopX>");
		sr.WriteLine ("<WindowTopY>0</WindowTopY>");
		sr.WriteLine ("<ProtectStructure>False</ProtectStructure>");
		sr.WriteLine ("<ProtectWindows>False</ProtectWindows>");
		sr.WriteLine ("</ExcelWorkbook>");
		sr.WriteLine ("<Styles>");
		sr.WriteLine ("<Style ss:ID=\"Default\" ss:Name=\"Normal\">");
		sr.WriteLine ("<Alignment ss:Vertical=\"Bottom\"/>");
		sr.WriteLine ("<Borders/>");
		sr.WriteLine ("<Font ss:FontName=\"Calibri\" x:Family=\"Swiss\" ss:Size=\"11\" ss:Color=\"#000000\"/>");
		sr.WriteLine ("<Interior/>");
		sr.WriteLine ("<NumberFormat/>");
		sr.WriteLine ("<Protection/>");
		sr.WriteLine ("</Style>");
		sr.WriteLine ("</Styles>");
		sr.WriteLine ("<Worksheet ss:Name=\"Sheet1\">");
		sr.WriteLine ("<Table ss:ExpandedColumnCount=\""+8+"\" ss:ExpandedRowCount=\""+recordedData.Count+"\" x:FullColumns=\"1\"");
		sr.WriteLine ("x:FullRows=\"1\" ss:DefaultRowHeight=\"15\">");
		for(int i=0;i<recordedData.Count;i++)
		{
			sr.WriteLine ("<Row ss:AutoFitHeight=\"0\">");
			sr.WriteLine ("<Cell><Data ss:Type=\"Number\">"+recordedData[i].fps+"</Data></Cell>");
			sr.WriteLine ("<Cell><Data ss:Type=\"Number\">"+recordedData[i].ms+"</Data></Cell>");
			sr.WriteLine ("<Cell><Data ss:Type=\"Number\">"+recordedData[i].float1+"</Data></Cell>");
			sr.WriteLine ("<Cell><Data ss:Type=\"Number\">"+recordedData[i].float2+"</Data></Cell>");
            sr.WriteLine("<Cell><Data ss:Type=\"Number\">" + recordedData[i].float3 + "</Data></Cell>");
            sr.WriteLine("<Cell><Data ss:Type=\"Number\">" + recordedData[i].float4 + "</Data></Cell>");
            sr.WriteLine("<Cell><Data ss:Type=\"Number\">" + recordedData[i].float5 + "</Data></Cell>");
            sr.WriteLine("<Cell><Data ss:Type=\"Number\">" + recordedData[i].method + "</Data></Cell>");
			sr.WriteLine ("</Row>");
		}
		sr.WriteLine ("</Table>");
		sr.WriteLine ("<WorksheetOptions xmlns=\"urn:schemas-microsoft-com:office:excel\">");
		sr.WriteLine ("<PageSetup>");
		sr.WriteLine ("<Header x:Margin=\"0.3\"/>");
		sr.WriteLine ("<Footer x:Margin=\"0.3\"/>");
		sr.WriteLine ("<PageMargins x:Bottom=\"0.75\" x:Left=\"0.7\" x:Right=\"0.7\" x:Top=\"0.75\"/>");
		sr.WriteLine ("</PageSetup>");
		sr.WriteLine ("<Unsynced/>");
		sr.WriteLine ("<Print>");
		sr.WriteLine ("<ValidPrinterInfo/>");
		sr.WriteLine ("<HorizontalResolution>600</HorizontalResolution>");
		sr.WriteLine ("<VerticalResolution>600</VerticalResolution>");
		sr.WriteLine ("</Print>");
		sr.WriteLine ("<Selected/>");
		sr.WriteLine ("<Panes>");
		sr.WriteLine ("<Pane>");
		sr.WriteLine ("<Number>3</Number>");
		sr.WriteLine ("<ActiveRow>7</ActiveRow>");
		sr.WriteLine ("<ActiveCol>5</ActiveCol>");
		sr.WriteLine ("</Pane>");
		sr.WriteLine ("</Panes>");
		sr.WriteLine ("<ProtectObjects>False</ProtectObjects>");
		sr.WriteLine ("<ProtectScenarios>False</ProtectScenarios>");
		sr.WriteLine ("</WorksheetOptions>");
		sr.WriteLine ("</Worksheet>");
		sr.WriteLine ("</Workbook>");
		sr.Close();
		Debug.Log("Performance successfully saved at "+System.DateTime.Now);
	}

}
