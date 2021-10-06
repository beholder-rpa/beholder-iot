terraform {
  required_providers {

    kubernetes = {
      source  = "hashicorp/kubernetes"
      version = "~>2.5.0"
    }

    helm = {
      source  = "hashicorp/helm"
      version = "~>2.3.0"
    }

    kubectl = {
      source  = "gavinbunney/kubectl"
      version = "~>1.11.3"
    }
  }
  required_version = "~> 1.0"
}