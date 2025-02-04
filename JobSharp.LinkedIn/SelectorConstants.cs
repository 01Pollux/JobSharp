namespace JobSharp.LinkedIn;

internal static class SelectorConstants
{
    public const string SignInForm = ".sign-in-form__sign-in-cta";
    public const string Container = ".scaffold-layout__list";
    public const string ChatPanel = ".msg-overlay-list-bubble";
    public const string ChatBubble = ".msg-overlay-conversation-bubble";
    public const string JobsPanel = "div.job-card-container";
    public const string Link = "a.job-card-container__link";
    public const string ApplyBtn = "button.jobs-apply-button[role='link']";
    public const string Title = ".artdeco-entity-lockup__title";
    public const string Company = ".artdeco-entity-lockup__subtitle";
    public const string CompanyLink = ".job-details-jobs-unified-top-card__company-name a";
    public const string Place = ".artdeco-entity-lockup__caption";
    public const string Date = "time";
    public const string DateText = ".job-details-jobs-unified-top-card__primary-description-container span:nth-of-type(3)";
    public const string Description = ".jobs-description";
    public const string DetailsPanel = ".jobs-search__job-details--container";
    public const string DetailsTop = ".jobs-details-top-card";
    public const string Details = ".jobs-details__main-content";
    public const string Insights = ".job-details-jobs-unified-top-card__container--two-pane li";
    public const string Pagination = ".jobs-search-two-pane__pagination";
    public const string PrivacyAcceptBtn = "button.artdeco-global-alert__action";
    public const string PaginationNextBtn = "li[data-test-pagination-page-btn].selected + li";
    public const string PaginationBtn = "ambda index: f'li[data-test-pagination-page-btn=\"{index}\"] button'";
    public const string RequiredSkills = ".job-details-how-you-match__skills-item-subtitle";
    public const string CancelEasyApply = ".artdeco-modal__dismiss";
    public const string ConfirmCancelEasyApply = ".artdeco-modal__confirm-dialog-btn";
}
