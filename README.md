# FreshCookees
C# .NET 3.5 tool that keeps proxy auth cookies fresh by maintaining a hidden IE process that navs to your hosted auto refresh page.  Uses WMI event listeners to monitor for InstanceDeletionEvents of the Internet Explorer process, and starts a hidden IE process via COM object if no other IE processes are running. 
