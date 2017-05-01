package com.vbondaryuk.drools;

import java.io.FileInputStream;
import java.io.InputStream;

import org.drools.core.util.IoUtils;
import org.drools.decisiontable.InputType;
import org.drools.decisiontable.SpreadsheetCompiler;
import org.drools.workbench.models.guided.dtable.backend.GuidedDTDRLPersistence;
import org.drools.workbench.models.guided.dtable.backend.GuidedDTXMLPersistence;
import org.drools.workbench.models.guided.dtable.shared.model.GuidedDecisionTable52;

public class DroolsRuleConverter {
	public static void main(String[] args) throws Exception {

	}

	public static String ConvertGdstToRule(String filePath) throws Exception {
		InputStream is = new FileInputStream(filePath);
		String decisionTableXml = new String(IoUtils.readBytesFromInputStream(is), IoUtils.UTF8_CHARSET);
		GuidedDTXMLPersistence guidedDTXMLPersistence = GuidedDTXMLPersistence.getInstance();
		GuidedDecisionTable52 guidedDecisionTable52 = guidedDTXMLPersistence.unmarshal(decisionTableXml);
		GuidedDTDRLPersistence guidedDTDRLPersistence = GuidedDTDRLPersistence.getInstance();
		String droolsRules = guidedDTDRLPersistence.marshal(guidedDecisionTable52);
		return droolsRules;
	}

	public static String ConvertXlsToRule(String filePath) throws Exception {
		InputStream is = new FileInputStream(filePath);
		SpreadsheetCompiler spreadsheetCompiller = new SpreadsheetCompiler();
		String drl = spreadsheetCompiller.compile(is, InputType.XLS);
		return drl;
	}

}
