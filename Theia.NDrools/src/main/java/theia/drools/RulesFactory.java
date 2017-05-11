package theia.drools;

import java.io.Reader;
import java.io.StringReader;
import java.util.Collection;
import java.util.HashSet;

import org.drools.KnowledgeBase;
import org.drools.KnowledgeBaseConfiguration;
import org.drools.KnowledgeBaseFactory;
import org.drools.builder.KnowledgeBuilder;
import org.drools.builder.KnowledgeBuilderConfiguration;
import org.drools.builder.KnowledgeBuilderErrors;
import org.drools.builder.KnowledgeBuilderFactory;
import org.drools.builder.ResourceType;
import org.drools.definition.KnowledgePackage;
import org.drools.io.Resource;
import org.drools.io.ResourceFactory;
import org.drools.runtime.StatefulKnowledgeSession;
import org.drools.runtime.rule.FactHandle;

public class RulesFactory {

	private KnowledgeBase kbase;
	private KnowledgeBuilder kbuilder;
	private StatefulKnowledgeSession ksession;
	private Collection<FactHandle> factHanldes;
	private KnowledgeBaseConfiguration kbaseConfiguration;
	private KnowledgeBuilderConfiguration kBuilderConfiguration;

	public RulesFactory()  throws Exception {
		this(new String[0]);
	}

	public RulesFactory(String[] assemblyQualifiedName) throws Exception {
		initFactory(assemblyQualifiedName);
	}

	private void initFactory(String[] assemblyQualifiedName) throws Exception {
		kbuilder = KnowledgeBuilderFactory.newKnowledgeBuilder();
		factHanldes = new HashSet<FactHandle>();

		ClassLoader[] classLoaders = new ClassLoader[assemblyQualifiedName.length];
		for (int i = 0; i < assemblyQualifiedName.length; i++) {
			classLoaders[i] = Class.forName(assemblyQualifiedName[i], false, ClassLoader.getSystemClassLoader())
					.getClassLoader();
		}
		kbaseConfiguration = KnowledgeBaseFactory.newKnowledgeBaseConfiguration(null, classLoaders);
		kbase = KnowledgeBaseFactory.newKnowledgeBase(kbaseConfiguration);	
		kBuilderConfiguration = KnowledgeBuilderFactory.newKnowledgeBuilderConfiguration(null, classLoaders);
		kbuilder = KnowledgeBuilderFactory.newKnowledgeBuilder(kBuilderConfiguration);
	}

	public RulesFactory AddRule(String source) {
		if (knowledgeSessionExists())
			throw new RuntimeException("После добавления объектов невозможно добавлять правила.");
		Resource myResource = ResourceFactory.newReaderResource((Reader) new StringReader(source));
		kbuilder.add(myResource, ResourceType.DRL);
		return this;
	}

	public RulesFactory AddObject(Object obj) {
		createKnowledgeSessionIfNotExists();
		FactHandle factHandle = ksession.insert(obj);
		factHanldes.add(factHandle);
		return this;
	}

	public RulesFactory RetractObjects() {
		
		for (KnowledgePackage knowledgePackage : kbuilder.getKnowledgePackages()) {
			kbase.removeKnowledgePackage(knowledgePackage.getName());
		}	

		ksession = null;
		factHanldes.clear();
		return this;
	}

	public RulesFactory FireAllRules() {
		knowledgeSessionExists();
		ksession.fireAllRules();
		RetractObjects();
		return this;
	}

	private void createKnowledgeSessionIfNotExists() {
		if (knowledgeSessionExists())
			return;
		kbase.addKnowledgePackages(kbuilder.getKnowledgePackages());
		ksession = kbase.newStatefulKnowledgeSession();
	}

	private Boolean knowledgeSessionExists() {
		return ksession != null;
	}

	public Boolean CheckKnowledgeBuilderError() {
		return kbuilder.getErrors().size() > 0;
	}

	public String GetKnowledgeBuilderErrors() {
		String errorString = "";
		KnowledgeBuilderErrors errors = kbuilder.getErrors();
		if (errors.size() > 0) {
			errorString += errors.toString();
		}
		return errorString;
	}
}
