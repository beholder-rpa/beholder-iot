### Provision Beholder Cortex (NextJS)
data "kubectl_path_documents" "beholder_cortex" {
  pattern = "${path.module}/specs/beholder_cortex.yaml"

  vars = {
    "cortex_hostname" = local.cortex_hostname
  }
}

resource "kubectl_manifest" "beholder_cortex" {
  count     = length(data.kubectl_path_documents.beholder_cortex.documents)
  yaml_body = element(data.kubectl_path_documents.beholder_cortex.documents, count.index)

  depends_on = [
    helm_release.traefik_ingress
  ]
}