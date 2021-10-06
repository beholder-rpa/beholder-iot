resource "kubernetes_namespace" "monitoring" {
  metadata {
    name = "monitoring"
  }
}

resource "helm_release" "prometheus" {
  name       = "prometheus"
  repository = "https://prometheus-community.github.io/helm-charts"
  chart      = "kube-prometheus-stack"
  version    = "19.0.2"

  namespace = "monitoring"

  values = [ 
    file("${path.module}/values/prometheus_values.yaml")
  ]

  set {
    name  = "alertmanager.config.clobal.smtp_from"
    value = local.beholder_from_email
  }

  set {
    name  = "receivers[1].email_configs[0].to"
    value = local.beholder_from_email
  }

  depends_on = [
    kubernetes_namespace.monitoring
  ]
}

### Provision promethus configuration
data "kubectl_path_documents" "prometheus" {
  pattern = "${path.module}/specs/prometheus_*.yaml"

  vars = {
    grafana_hostname = local.grafana_hostname
  }
}

resource "kubectl_manifest" "prometheus" {
  count     = length(data.kubectl_path_documents.prometheus.documents)
  yaml_body = element(data.kubectl_path_documents.prometheus.documents, count.index)
  depends_on = [
    helm_release.prometheus
  ]
}
