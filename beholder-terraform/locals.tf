locals {
  ###########
  # Hostnames

  cortex_hostname = "cortex.${var.hostname}"
  nexus_hostname = "nexus.${var.hostname}"

  whoami_hostname = "whoami.${var.hostname}"
  dapr_dashboard_hostname = "dapr.${var.hostname}"
  traefik_dashboard_hostname = "traefik.${var.hostname}"

  traefik_additional_arguments = [
    "--ping=true",
    "--global.checkNewVersion=true",
    "--global.sendAnonymousUsage=false",
    "--api.dashboard=true",
    "--entrypoints.amqp.address=:5672",
    "--entrypoints.mqtt.address=:1883",
    "--entrypoints.redis.address=:6379",
  ]
}