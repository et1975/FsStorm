#I "../../Refs"
#I "../../packages/Thrift/lib/net35"
#load "StormSubmit.fsx"

let binDir = "src/FstGuaranteed/bin/Release"

StormSubmit.runTopology binDir "localhost" StormSubmit.default_nimbus_port
