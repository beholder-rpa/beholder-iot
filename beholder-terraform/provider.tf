provider "kubernetes" {
  config_paths = [
    "/etc/rancher/k3s/k3s.yaml",
    "~/.kube/config"
  ]
  config_context = "default"
}

provider "kubectl" {
  config_paths = [
    "/etc/rancher/k3s/k3s.yaml",
    "~/.kube/config"
  ]
  config_context = "default"
}

provider "helm" {
  kubernetes {
    config_paths = [
    "/etc/rancher/k3s/k3s.yaml",
    "~/.kube/config"
    ]
    config_context = "default"
  }
}
