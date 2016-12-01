using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using DTADataImportWindowsService;


namespace DTADataImportWindowsService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
            this.DTADataImportWindowsService.ServiceName = ServiceInstallLoad.ServiceName;
           this.DTADataImportWindowsService.DisplayName = ServiceInstallLoad.DisplayName;
            this.DTADataImportWindowsService.Description = ServiceInstallLoad.Description;
        }
    }
}
