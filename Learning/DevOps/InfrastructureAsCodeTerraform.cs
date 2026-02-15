// ==============================================================================
// Infrastructure as Code with Terraform
// ==============================================================================
// WHAT IS THIS?
// Terraform is an IaC tool that provisions cloud resources (servers, databases, networks) via code. Write HCL configuration, Terraform creates/updates/destroys infrastructure.
//
// WHY IT MATTERS
// ✅ VERSION CONTROL: Infrastructure in Git, history of changes | ✅ REPRODUCIBLE: Same code = same infrastructure every time | ✅ IDEMPOTENT: Run multiple times, same result | ✅ AUDITABLE: Know who changed what when | ✅ DESTROY: Tear down entire environment with one command
//
// WHEN TO USE
// ✅ Cloud infrastructure (AWS, Azure, GCP) | ✅ Multi-environment (dev, staging, prod) | ✅ Need disaster recovery | ✅ Team collaboration on infrastructure
//
// WHEN NOT TO USE
// ❌ Manual one-off deployments | ❌ Complex custom logic (use scripts) | ❌ Limited infrastructure needs
//
// REAL-WORLD EXAMPLE
// E-commerce: Terraform defines 50-VM cluster, RDS database, load balancer, networking. Dev pushes code. Pipeline runs terraform plan, shows changes. Approval triggers apply. 5 minutes later, prod deployed.
// ==============================================================================

using System;
using System.Collections.Generic;

namespace RevisionNotesDemo.DevOps;

public class InfrastructureAsCodeTerraform
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Infrastructure as Code with Terraform");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝\n");
        
        Overview();
        HCLSyntax();
        TerraformWorkflow();
        StateManagement();
        BestPractices();
    }

    private static void Overview()
    {
        Console.WriteLine("📖 OVERVIEW:\n");
        Console.WriteLine("Terraform = cloud resource provisioning automation");
        Console.WriteLine("Supports: AWS, Azure, GCP, Kubernetes, etc.");
        Console.WriteLine("Workflow: init → plan → apply → destroy\n");
    }

    private static void HCLSyntax()
    {
        Console.WriteLine("💚 HCL SYNTAX EXAMPLE:\n");
        
        Console.WriteLine("# Configure AWS provider");
        Console.WriteLine("provider \\\"aws\\\" {");
        Console.WriteLine("  region = \\\"us-east-1\\\"");
        Console.WriteLine("}\n");
        
        Console.WriteLine("# Create EC2 instance");
        Console.WriteLine("resource \\\"aws_instance\\\" \\\"web\\\" {");
        Console.WriteLine("  instance_type = \\\"t3.medium\\\"");
        Console.WriteLine("  ami           = \\\"ami-0c55b159cbfafe1f0\\\"");
        Console.WriteLine("  key_name      = \\\"my-key\\\"");
        Console.WriteLine("}\n");
        
        Console.WriteLine("# Output instance IP");
        Console.WriteLine("output \\\"instance_ip\\\" {");
        Console.WriteLine("  value = aws_instance.web.public_ip");
        Console.WriteLine("}\n");
    }

    private static void TerraformWorkflow()
    {
        Console.WriteLine("🔄 WORKFLOW:\n");
        
        Console.WriteLine("1. terraform init");
        Console.WriteLine("   Downloads AWS provider, initializes state\n");
        
        Console.WriteLine("2. terraform plan");
        Console.WriteLine("   Compares desired (code) vs current (state)");
        Console.WriteLine("   Shows changes: + create, ~ update, - delete\n");
        
        Console.WriteLine("3. terraform apply");
        Console.WriteLine("   Executes changes");
        Console.WriteLine("   Updates state file\n");
        
        Console.WriteLine("4. terraform destroy");
        Console.WriteLine("   Removes all resources\n");
    }

    private static void StateManagement()
    {
        Console.WriteLine("📦 STATE MANAGEMENT:\n");
        
        Console.WriteLine("State file (terraform.tfstate) tracks current state:");
        Console.WriteLine("  {");
        Console.WriteLine("    \\\"resource\\\": [{");
        Console.WriteLine("      \\\"type\\\": \\\"aws_instance\\\",");
        Console.WriteLine("      \\\"id\\\": \\\"i-1234567890abcdef0\\\",");
        Console.WriteLine("      \\\"attributes\\\": { \\\"public_ip\\\": \\\"1.2.3.4\\\" }");
        Console.WriteLine("    }]");
        Console.WriteLine("  }\n");
        
        Console.WriteLine("⚠️  IMPORTANT:");
        Console.WriteLine("  - Never commit to Git (contains secrets!)");
        Console.WriteLine("  - Store in S3/Terraform Cloud (with locking)\n");
    }

    private static void BestPractices()
    {
        Console.WriteLine("✨ BEST PRACTICES:\n");
        
        Console.WriteLine("1. VARIABLE MANAGEMENT");
        Console.WriteLine("   variables.tf: Define variables");
        Console.WriteLine("   terraform.tfvars: Provide values\n");
        
        Console.WriteLine("2. MODULES (Reusability)");
        Console.WriteLine("   modules/vpc/");
        Console.WriteLine("   modules/database/");
        Console.WriteLine("   Use in main configuration\n");
        
        Console.WriteLine("3. ENVIRONMENTS");
        Console.WriteLine("   terraform/dev/");
        Console.WriteLine("   terraform/staging/");
        Console.WriteLine("   terraform/prod/\n");
        
        Console.WriteLine("4. PLAN BEFORE APPLY");
        Console.WriteLine("   Always review plan output");
        Console.WriteLine("   Catch destructive changes\n");
    }
}
