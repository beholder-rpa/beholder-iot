resource "helm_release" "bitnami_redis" {
  name       = "beholder-redis"
  repository = "https://charts.bitnami.com/bitnami"
  chart      = "redis"

  values = [ 
    file("${path.module}/values/redis_values.yaml")
  ]
}