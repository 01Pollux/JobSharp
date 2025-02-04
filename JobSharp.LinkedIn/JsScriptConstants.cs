namespace JobSharp.LinkedIn;

internal class JsScriptConstants
{
    public const string FetchJobDetails = $@"
        const index = arguments[0];
        const allJobs = document.querySelectorAll('{SelectorConstants.JobsPanel}');
        if (index >= allJobs.length) {{
            return null;
        }}

        const job = allJobs[index];
        const link = job.querySelector('{SelectorConstants.Link}');
        if (link === null) {{
            throw new Error('Job link not found');
        }}
        
        // Click job link and scroll
        link.scrollIntoView({{ behavior: 'smooth', block: 'center' }});
        link.click();
        
        // Extract job link (relative)
        const protocol = window.location.protocol + ""//"";
        const hostname = window.location.hostname;
        const jobUrl = protocol + hostname + link.getAttribute(""href"");                                                            
        
        const jobId = job.getAttribute(""data-job-id"");
        
        let jobTitle = job.querySelector('{SelectorConstants.Title}') ?
            job.querySelector('{SelectorConstants.Title}').innerText : """";
        
        if (jobTitle.includes('\\n')) {{
            jobTitle = jobTitle.split('\\n')[1];
        }}
            
        let company = """";                                
        const companyElem = job.querySelector('{SelectorConstants.Company}'); 
        
        if (companyElem) {{                                    
            company = companyElem.innerText;                                                                        
        }}
        
        const companyImgLinkElement = job.querySelector(""img"");
        const companyImgLink = companyImgLinkElement ? companyImgLinkElement.getAttribute(""src"") : """";
        
        const companyLinkElement = document.querySelector('{SelectorConstants.CompanyLink}');
        const companyLink = companyLinkElement ? companyLinkElement.getAttribute(""href"") : """";
        
        let jobLocation = job.querySelector('{SelectorConstants.Place}') ?
            job.querySelector('{SelectorConstants.Place}').innerText : """";
        
        const jobDateElement = document.querySelector('{SelectorConstants.DateText}');
        const jobDate = jobDateElement ? jobDateElement.innerText : """";
            
        const descriptionElement = document.querySelector('{SelectorConstants.Description}');
        const description = descriptionElement ? descriptionElement.innerText : """";

        const isPromoted = Array.from(job.querySelectorAll('li'))
            .find(e => e.innerText === 'Promoted') ? true : false;

        // normalize whitespace from title, company, location (\n, \t, \r) to space
        jobTitle = jobTitle.replace(/\s+/g, ' ').trim();
        company = company.replace(/\s+/g, ' ').trim();
        jobLocation = jobLocation.replace(/\s+/g, ' ').trim();

        const obj = {{
            JobId: jobId,
            Title: jobTitle,
            Description: description,
            Company: company,                                    
            CompanyLink: companyLink,
            CompanyImageLink: companyImgLink,
            Location: jobLocation,
            Url: jobUrl,
            PostDate: jobDate,
            IsPromoted: isPromoted,
        }};

        return JSON.stringify(obj);
";
}
