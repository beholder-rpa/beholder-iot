### Provision Beholder Prefrontal (Redis)
data "kubectl_path_documents" "beholder_prefrontal" {
  pattern = "${path.module}/specs/beholder_prefrontal.yaml"
}

resource "kubectl_manifest" "beholder_prefrontal" {
  count     = length(data.kubectl_path_documents.beholder_prefrontal.documents)
  yaml_body = element(data.kubectl_path_documents.beholder_prefrontal.documents, count.index)
}