namespace NECInspector.Codes
{
    /// <summary>
    /// What the scenario list says when it has nothing to show, so an empty list is never unexplained.
    /// Plain text, so the wording is tested without Unity.
    /// </summary>
    public static class ScenarioListMessage
    {
        /// <summary>The message for the active code, or an empty string when scenarios are listed.</summary>
        public static string For(ICodeProfile profile, int listedScenarios)
        {
            if (profile == null)
                return "No installation code is loaded, so no scenarios can be shown.";

            if (!ArtSets.IsAvailable(profile.ArtSet))
                return $"Scenarios are not available for {profile.DisplayName} yet: there is no scene art for its region. " +
                       "You can still study its references.";

            if (listedScenarios <= 0)
                return $"No scenarios cover {profile.DisplayName} yet.";

            return "";
        }
    }
}
