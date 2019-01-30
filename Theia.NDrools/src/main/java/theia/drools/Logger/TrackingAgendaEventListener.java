package theia.drools.Logger;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;

import org.drools.definition.rule.Rule;
import org.drools.event.rule.DefaultAgendaEventListener;
import org.drools.event.rule.AfterActivationFiredEvent;

/**
 * A listener that will track all rule firings in a session.
 * 
 * @author Stephen Masters
 */
public class TrackingAgendaEventListener extends DefaultAgendaEventListener {

	public TrackingAgendaEventListener(IDroolsLogger logger) {
		_logger = logger;
	}
	
    private IDroolsLogger _logger;

    private List<Activation> activationList = new ArrayList<Activation>();

    @Override
    public void afterActivationFired(AfterActivationFiredEvent event) {
        Rule rule = event.getActivation().getRule();

        String ruleName = rule.getName();
        Map<String, Object> ruleMetaDataMap = rule.getMetaData();

        activationList.add(new Activation(ruleName));
        StringBuilder sb = new StringBuilder("Rule fired: " + ruleName);

        if (ruleMetaDataMap.size() > 0) {
            sb.append("\n  With [" + ruleMetaDataMap.size() + "] meta-data:");
            for (String key : ruleMetaDataMap.keySet()) {
                sb.append("\n    key=" + key + ", value=" + ruleMetaDataMap.get(key));
            }
        }

        _logger.debug(sb.toString());
    }

    public boolean isRuleFired(String ruleName) {
        for (Activation a : activationList) {
            if (a.getRuleName().equals(ruleName)) {
                return true;
            }
        }
        return false;
    }

    public void reset() {
        activationList.clear();
    }

    public final List<Activation> getActivationList() {
        return activationList;
    }

    public String activationsToString() {
        if (activationList.size() == 0) {
            return "No activations occurred.";
        } else {
            StringBuilder sb = new StringBuilder("Activations: ");
            for (Activation activation : activationList) {
                sb.append("\n  rule: ").append(activation.getRuleName());
            }
            return sb.toString();
        }
    }

}
