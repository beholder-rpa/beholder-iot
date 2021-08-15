Beholder Occipital v1
---

Services dealing with visual/image processing

Uses Dapr via GPRC protocol

When Developing locally, the following hosts file entries are required to point to the locally running container instances:

```
127.0.0.1 beholder-nexus
127.0.0.1 beholder-redis
127.0.0.1 jaeger
```

For updates to the pattern, see: 
See https://github.com/dapr/dotnet-sdk/tree/master/examples/AspNetCore/GrpcServiceSample

Also:
https://docs.microsoft.com/en-us/cpp/build/reference/common-macros-for-build-commands-and-properties?view=msvc-160&viewFallbackFrom=vs-2017