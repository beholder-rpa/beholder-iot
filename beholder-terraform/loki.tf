resource "helm_release" "loki" {
  name       = "loki"
  repository = "https://grafana.github.io/helm-charts"
  chart      = "loki-stack"

  namespace = "monitoring"

  values = [ 
    file("${path.module}/values/loki_values.yaml")
  ]

  depends_on = [
    kubernetes_namespace.monitoring
  ]
}