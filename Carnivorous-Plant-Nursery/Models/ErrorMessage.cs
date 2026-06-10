namespace Carnivorous_Plant_Nursery.Models
{
    public static class ErrorMessage
    {
        public const string NegativeEstimatedAge = "Estimated age can not be negative.";
        public const string NegativeSeedCount = "Seed count can not be negative.";
        public const string InvalidGerminationRate = "Germination rate must be between 0.0 and 1.0.";
        public const string NegativePrice = "Price can not be negative.";
        public const string NegativePotDiameter = "Pot diameter can not be negative.";
        public const string NegativePotHeight = "Pot height can not be negative.";
        public const string DeleteErrorPlantInLineage = "This plant cannot be deleted because it is recorded as a parent in one or more lineage entries. Remove those lineage records first.";
        public const string DeleteErrorSeedBatchInLineage = "This seed batch cannot be deleted because it is recorded as a parent in one or more lineage entries. Remove those lineage records first.";
        public const string DeleteErrorSeedBatchHasPlants = "This seed batch cannot be deleted because one or more plants were sourced from it. Remove or reassign those plant records first.";
        public const string DeleteErrorTaxonomyHasItems = "This taxonomy cannot be deleted because plants or seed batches are assigned to it. Remove or reassign those records first.";
        public const string DeleteErrorCareProfileHasTaxonomies = "This care profile cannot be deleted because one or more taxonomies are assigned to it. Remove or reassign those taxonomies first.";
        public const string ApiInvalidReference = "One or more referenced records do not exist.";
        public const string InvalidLoginAttempt = "Invalid login attempt.";
        public const string InvalidPhoneNumberFormat = "Phone number format is not valid.";
        public const string InvalidCityFormat = "City can contain only letters, spaces, apostrophes, periods, and hyphens.";
        public const string ExternalLoginError = "External login could not be completed.";
        public const string ExternalLoginEmailMissing = "External login did not provide an email address.";
        public const string AccountUpdateFailed = "Account details could not be updated.";
        public const string CurrentPasswordRequired = "Current password is required to change your password.";
        public const string NewPasswordRequired = "New password is required when current password is provided.";
        public const string LocalPasswordUnavailable = "This account does not have a local password to change.";
        public const string PasswordMinimumLength = "Password must be at least 8 characters long.";
        public const string PasswordConfirmationMismatch = "The password and confirmation password do not match.";
        public const string AttachmentFileRequired = "Select at least one image before uploading.";
        public const string AttachmentInvalidImageType = "Only JPG, PNG, WEBP, and GIF image files can be uploaded.";
        public const string AttachmentTooLarge = "Uploaded images must be 5 MB or smaller.";
        public const string AttachmentPlantNotFound = "The selected plant does not exist.";
        public const string AttachmentSeedBatchNotFound = "The selected seed batch does not exist.";
        public const string AttachmentPendingFileMissing = "One or more uploaded images could not be found. Upload them again before saving.";
    }
}
