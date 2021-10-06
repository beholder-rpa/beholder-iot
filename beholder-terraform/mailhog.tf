
resource "helm_release" "mailhog" {
  name       = "mailhog"
  repository = "https://codecentric.github.io/helm-charts"
  chart      = "mailhog"
  version    = "5.0.1"

  namespace = "default"

  values = [ 
    file("${path.module}/values/mailhog_values.yaml")
  ]
}

data "kubectl_path_documents" "mailhog" {
  pattern = "${path.module}/specs/mailhog.yaml"

  vars = {
    mailhog_hostname = local.webmail_hostname
  }
}

resource "kubectl_manifest" "mailhog" {
  count     = length(data.kubectl_path_documents.mailhog.documents)
  yaml_body = element(data.kubectl_path_documents.mailhog.documents, count.index)

  depends_on = [
    helm_release.mailhog
  ]
}