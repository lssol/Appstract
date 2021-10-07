mkdir -p appstract/__rpc/ -ea 0
python -m grpc_tools.protoc -I../ --python_out=appstract/__rpc --grpc_python_out=appstract/__rpc ../appstract.proto