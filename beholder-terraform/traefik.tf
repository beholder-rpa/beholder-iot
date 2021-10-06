### Provision Traefik
// https://github.com/traefik/traefik-helm-chart
resource "helm_release" "traefik_ingress" {
  name       = "traefik"
  repository = "https://helm.traefik.io/traefik"
  chart      = "traefik"
  version    = "10.3.6"

  namespace = "kube-system"

  values = [ 
    file("${path.module}/values/traefik_values.yaml")
  ]

  set {
    name  = "additionalArguments"
    value = "{${join(",", local.traefik_additional_arguments)}}"
  }
}

### Provision Traefik base configuration
data "kubectl_path_documents" "traefik_base" {
  pattern = "${path.module}/specs/traefik_*.yaml"

  vars = {
    traefik_dashboard_hostname = local.traefik_dashboard_hostname
  }
}

resource "kubectl_manifest" "traefik_base" {
  count     = length(data.kubectl_path_documents.traefik_base.documents)
  yaml_body = element(data.kubectl_path_documents.traefik_base.documents, count.index)
  depends_on = [
    helm_release.traefik_ingress
  ]
}