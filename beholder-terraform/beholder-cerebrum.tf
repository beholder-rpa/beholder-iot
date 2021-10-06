### Provision Beholder Cerebrum (NodeRed)
data "kubectl_path_documents" "beholder_cerebrum" {
  pattern = "${path.module}/specs/beholder_cerebrum.yaml"

  vars = {
    "cerebrum_hostname" = local.cerebrum_hostname
  }
}

resource "kubectl_manifest" "beholder_cerebrum" {
  count     = length(data.kubectl_path_documents.beholder_cerebrum.documents)
  yaml_body = element(data.kubectl_path_documents.beholder_cerebrum.documents, count.index)

  depends_on = [
    helm_release.traefik_ingress
  ]
}