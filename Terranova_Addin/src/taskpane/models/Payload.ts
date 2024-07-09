import Configuration from "./Configuration";
import Simulation from "./Simulation";
import TypeSelection from "./TypeSelection";

export default class Payload {
  mailItemId: string;
  feedbackMessage: string;
  environmentId: string;
  mailboxAddress: string;
  simulationInfo: Simulation;
  configurationInfo: Configuration;
  emailType: TypeSelection;
}
