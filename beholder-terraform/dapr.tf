# Dapr Helm Chart
resource "helm_release" "dapr" {
  name       = "beholder-dapr"
  repository = "https://dapr.github.io/helm-charts/"
  chart      = "dapr"
}

### Dapr Configuration
data "kubectl_path_documents" "dapr_config" {
  pattern = "${path.module}/specs/dapr_*.yaml"

  vars = {
    "dapr_dashboard_hostname" = local.dapr_dashboard_hostname
  }
}

resource "kubectl_manifest" "dapr_config" {
  count     = length(data.kubectl_path_documents.dapr_config.documents)
  yaml_body = element(data.kubectl_path_documents.dapr_config.documents, count.index)
}