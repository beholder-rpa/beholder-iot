# Dapr Helm Chart
resource "helm_release" "dapr" {
  name       = "beholder-dapr"
  repository = "https://dapr.github.io/helm-charts/"
  chart      = "dapr"

  namespace = local.dapr_namespace
}

### Dapr Configuration
data "kubectl_path_documents" "app_dapr_config" {
  pattern = "${path.module}/specs/dapr_*.yaml"

  vars = {
    dapr_namespace = local.dapr_namespace
  }
}