#I "../../Refs"
#I "../../packages/Thrift/lib/net35"
#load "StormSubmit.fsx"

let binDir = "src/FstSample/bin/Release"

StormSubmit.runTopology binDir "localhost" StormSubmit.default_nimbus_port
