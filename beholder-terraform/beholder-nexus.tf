### Provision Beholder Nexus (Emqx)
data "kubectl_path_documents" "beholder_nexus" {
  pattern = "${path.module}/specs/beholder_nexus.yaml"

  vars = {
    "nexus_hostname" = local.nexus_hostname
    "cortex_hostname" = local.cortex_hostname
  }
}

resource "kubectl_manifest" "beholder_nexus" {
  count     = length(data.kubectl_path_documents.beholder_nexus.documents)
  yaml_body = element(data.kubectl_path_documents.beholder_nexus.documents, count.index)

  depends_on = [
    helm_release.traefik_ingress
  ]
}