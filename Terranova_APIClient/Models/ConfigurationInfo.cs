using System;
using System.Collections.Generic;
using System.Text;

namespace Terranova_APIClient.Models
{
    public class ConfigurationInfo
    {
        private string _userAuthorizationDescriptionText = "You need to authorize Terranova Security phishing add-in before you can report this email";
        private string _authorizeButtonForegroundColor = "#ffffff";
        private string _authorizeButtonBackgroundColor = "#005a9e";
        private string _userAuthorizationButtonText = "Authorize";
        private string _nextConfirmationMessageButtonText = "Next";
        private string _nextConfirmationMessageButtonBackgroundColor = "#005a9e";
        private string _nextConfirmationMessageButtonForegroundColor = "#ffffff";
        private string _nextTypeAndCommentButtonText = "Next";
        private string _nextTypeAndCommentButtonBackgroundColor = "#005a9e";
        private string _nextTypeAndCommentButtonForegroundColor = "#ffffff";
        private string _moveButtonForegroundColor = "#ffffff";
        private string _moveButtonBackgroundColor = "#005a9e";
        private string _movedToDeletedItemsText = "Email moved to deleted items. You can close the email now.";
        private string _movedToJunkText = "Email moved to junk. You can close the email now.";
        private string _authorizingLabelText = "Authorizing...";
        private string _validatingAuthText = "Validating Auth...";
        private string _reportingText = "Reporting...";
        private string _movingText= "Moving...";
        private string _loadingLabelText = "Loading...";

        public int VersionNumber { get; set; }
        public bool DisplayConfirmation { get; set; }
        public bool PromptUserForTypeSelection { get; set; }
        public List<TypeSelectionInfo> TypeSelectionButtonCaptions { get; set; }
        public string TypeSelectionPromptMessageCaption { get; set; }
        public bool CommentBoxEnabled { get; set; }
        public string CommentBoxCaption { get; set; }
        public string SimulationReportFeedbackMessage { get; set; }
        public bool SimulationIncidentResponseEnabled { get; set; }
        public string SimulationIncidentResponseEmail { get; set; }
        public string ReportFeedbackMessage { get; set; }
        public bool IncidentResponseEnabled { get; set; }
        public string IncidentResponseEmail { get; set; }
        public bool BccForwardingEnbled { get; set; }
        public string BccForwardingEmail { get; set; }
        public bool MicrosoftReportingEnabled { get; set; }
        public string ConfirmationPromptMessageCaption { get; set; }
        public string NextButtonCaption { get; set; }
        public string MoveToJunkButtonCaption { get; set; }
        public string MoveToDeletedItemsButtonCaption { get; set; }
        public bool BccForwardingSimulatedEmail { get; set; } = false;
        public bool BccForwardingNonSimulatedEmail { get; set; } = true;
        public bool DisplaySimulationReinforcementMessage { get; set; } = true;
        public bool MoveNonSimulationEmailsToJunk { get; set; } = true;
        public bool MoveSimulationEmailsToJunk { get; set; } = false;
        public bool ForwardEmailAsAttachment { get; set; } = true;
        public bool UserAuthorizationScreenDisplay { get; set; } = true;
        public bool DisplayFeedbackScreen { get; set; } = true;
        public bool FormatTitleForOffice365SecurityReportSubmission { get; set; } = false;
        public bool AttachEmailWithOriginalTitle { get; set; } = false;

        public string UserAuthorizationDescriptionText
        {
            get
            {
                return _userAuthorizationDescriptionText;
            }
            set
            {
                _userAuthorizationDescriptionText = string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value) ? _userAuthorizationDescriptionText : value;
            }
        }
        public string AuthorizeButtonForegroundColor
        {
            get
            {
                return _authorizeButtonForegroundColor;
            }
            set
            {
                _authorizeButtonForegroundColor = string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value) ? _authorizeButtonForegroundColor : value;
            }
        }
        public string AuthorizeButtonBackgroundColor
        {
            get
            {
                return _authorizeButtonBackgroundColor;
            }
            set
            {
                _authorizeButtonBackgroundColor = string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value) ? _authorizeButtonBackgroundColor : value;
            }
        }
        public string UserAuthorizationButtonText
        {
            get
            {
                return _userAuthorizationButtonText;
            }
            set
            {
                _userAuthorizationButtonText = string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value) ? _userAuthorizationButtonText : value;
            }
        }
        public string NextConfirmationMessageButtonText
        {
            get
            {
                return _nextConfirmationMessageButtonText;
            }
            set
            {
                _nextConfirmationMessageButtonText = string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value) ? _nextConfirmationMessageButtonText : value;
            }
        }
        public string NextConfirmationMessageButtonBackgroundColor
        {
            get
            {
                return _nextConfirmationMessageButtonBackgroundColor;
            }
            set
            {
                _nextConfirmationMessageButtonBackgroundColor = string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value) ? _nextConfirmationMessageButtonBackgroundColor : value;
            }
        }
        public string NextConfirmationMessageButtonForegroundColor
        {
            get
            {
                return _nextConfirmationMessageButtonForegroundColor;
            }
            set
            {
                _nextConfirmationMessageButtonForegroundColor = string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value) ? _nextConfirmationMessageButtonForegroundColor : value;
            }
        }
        public string NextTypeAndCommentButtonText
        {
            get
            {
                return _nextTypeAndCommentButtonText;
            }
            set
            {
                _nextTypeAndCommentButtonText = string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value) ? _nextTypeAndCommentButtonText : value;
            }
        }
        public string NextTypeAndCommentButtonBackgroundColor
        {
            get
            {
                return _nextTypeAndCommentButtonBackgroundColor;
            }
            set
            {
                _nextTypeAndCommentButtonBackgroundColor = string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value) ? _nextTypeAndCommentButtonBackgroundColor : value;
            }
        }
        public string NextTypeAndCommentButtonForegroundColor
        {
            get
            {
                return _nextTypeAndCommentButtonForegroundColor;
            }
            set
            {
                _nextTypeAndCommentButtonForegroundColor = string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value) ? _nextTypeAndCommentButtonForegroundColor : value;
            }
        }
        public string MoveButtonForegroundColor
        {
            get
            {
                return _moveButtonForegroundColor
;
            }
            set
            {
                _moveButtonForegroundColor = string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value) ? _moveButtonForegroundColor : value;
            }
        }
        public string MoveButtonBackgroundColor
        {
            get
            {
                return _moveButtonBackgroundColor
;
            }
            set
            {
                _moveButtonBackgroundColor = string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value) ? _moveButtonBackgroundColor : value;
            }
        }
        public string MovedToDeletedItemsText
        {
            get
            {
                return _movedToDeletedItemsText
;
            }
            set
            {
                _movedToDeletedItemsText = string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value) ? _movedToDeletedItemsText : value;
            }
        }
        public string MovedToJunkText
        {
            get
            {
                return _movedToJunkText
;
            }
            set
            {
                _movedToJunkText = string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value) ? _movedToJunkText : value;
            }
        }
        public string AuthorizingLabelText
        {
            get
            {
                return _authorizingLabelText;
            }
            set
            {
                _authorizingLabelText = string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value) ? _authorizingLabelText : value;
            }
        }
        public string ValidatingAuthText
        {
            get
            {
                return _validatingAuthText;
            }
            set
            {
                _validatingAuthText = string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value) ? _validatingAuthText : value;
            }
        }
        public string ReportingText
        {
            get
            {
                return _reportingText;
            }
            set
            {
                _reportingText = string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value) ? _reportingText : value;
            }
        }
        public string MovingText
        {
            get
            {
                return _movingText;
            }
            set
            {
                _movingText = string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value) ? _movingText : value;
            }
        }
        public string LoadingLabelText
        {
            get
            {
                return _loadingLabelText;
            }
            set
            {
                _loadingLabelText = string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value) ? _loadingLabelText : value;
            }
        }
    }
}
