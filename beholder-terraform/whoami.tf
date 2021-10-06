data "kubectl_path_documents" "whoami" {
  pattern = "${path.module}/specs/whoami.yaml"

  vars = {
    whoami_hostname = local.whoami_hostname
  }
}

resource "kubectl_manifest" "whoami" {
  count     = length(data.kubectl_path_documents.whoami.documents)
  yaml_body = element(data.kubectl_path_documents.whoami.documents, count.index)
}