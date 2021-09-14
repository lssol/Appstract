# -*- coding: utf-8 -*-
# Generated by the protocol buffer compiler.  DO NOT EDIT!
# source: appstract.proto
"""Generated protocol buffer code."""
from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from google.protobuf import reflection as _reflection
from google.protobuf import symbol_database as _symbol_database
# @@protoc_insertion_point(imports)

_sym_db = _symbol_database.Default()




DESCRIPTOR = _descriptor.FileDescriptor(
  name='appstract.proto',
  package='proto_appstract',
  syntax='proto3',
  serialized_options=None,
  create_key=_descriptor._internal_create_key,
  serialized_pb=b'\n\x0f\x61ppstract.proto\x12\x0fproto_appstract\"\x07\n\x05\x45mpty\"$\n\x04Page\x12\x0f\n\x07\x63ontent\x18\x01 \x01(\t\x12\x0b\n\x03url\x18\x02 \x01(\t\"C\n\x07\x43luster\x12$\n\x05pages\x18\x01 \x03(\x0b\x32\x15.proto_appstract.Page\x12\x12\n\nconfidence\x18\x02 \x01(\x01\"b\n\x17\x43reateClusteringRequest\x12\x11\n\tclusterer\x18\x01 \x01(\t\x12\x0e\n\x06\x64omain\x18\x02 \x01(\t\x12$\n\x05pages\x18\x03 \x03(\x0b\x32\x15.proto_appstract.Page\"T\n\x12\x43lusteringResponse\x12*\n\x08\x63lusters\x18\x01 \x03(\x0b\x32\x18.proto_appstract.Cluster\x12\x12\n\nconfidence\x18\x02 \x01(\x01\" \n\x0f\x43lusterersReply\x12\r\n\x05label\x18\x01 \x01(\t\" \n\x0e\x45xploreRequest\x12\x0e\n\x06\x64omain\x18\x01 \x01(\t\"}\n\x0c\x45xploreReply\x12\x0f\n\x07\x63ontent\x18\x01 \x01(\t\x12\x0b\n\x03url\x18\x02 \x01(\t\x12\x0e\n\x06\x64omain\x18\x03 \x01(\t\x12\x0e\n\x06origin\x18\x04 \x01(\t\x12\x0f\n\x07nbNodes\x18\x05 \x01(\x05\x12\x0f\n\x07nbLinks\x18\x06 \x01(\x05\x12\r\n\x05\x65rror\x18\x07 \x01(\x08\x32\xbe\x01\n\nClustering\x12\x63\n\x10\x43reateClustering\x12(.proto_appstract.CreateClusteringRequest\x1a#.proto_appstract.ClusteringResponse\"\x00\x12K\n\rGetClusterers\x12\x16.proto_appstract.Empty\x1a .proto_appstract.ClusterersReply\"\x00\x32V\n\x05Robot\x12M\n\x07\x45xplore\x12\x1f.proto_appstract.ExploreRequest\x1a\x1d.proto_appstract.ExploreReply\"\x00\x30\x01\x62\x06proto3'
)




_EMPTY = _descriptor.Descriptor(
  name='Empty',
  full_name='proto_appstract.Empty',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  create_key=_descriptor._internal_create_key,
  fields=[
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=36,
  serialized_end=43,
)


_PAGE = _descriptor.Descriptor(
  name='Page',
  full_name='proto_appstract.Page',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  create_key=_descriptor._internal_create_key,
  fields=[
    _descriptor.FieldDescriptor(
      name='content', full_name='proto_appstract.Page.content', index=0,
      number=1, type=9, cpp_type=9, label=1,
      has_default_value=False, default_value=b"".decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='url', full_name='proto_appstract.Page.url', index=1,
      number=2, type=9, cpp_type=9, label=1,
      has_default_value=False, default_value=b"".decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=45,
  serialized_end=81,
)


_CLUSTER = _descriptor.Descriptor(
  name='Cluster',
  full_name='proto_appstract.Cluster',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  create_key=_descriptor._internal_create_key,
  fields=[
    _descriptor.FieldDescriptor(
      name='pages', full_name='proto_appstract.Cluster.pages', index=0,
      number=1, type=11, cpp_type=10, label=3,
      has_default_value=False, default_value=[],
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='confidence', full_name='proto_appstract.Cluster.confidence', index=1,
      number=2, type=1, cpp_type=5, label=1,
      has_default_value=False, default_value=float(0),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=83,
  serialized_end=150,
)


_CREATECLUSTERINGREQUEST = _descriptor.Descriptor(
  name='CreateClusteringRequest',
  full_name='proto_appstract.CreateClusteringRequest',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  create_key=_descriptor._internal_create_key,
  fields=[
    _descriptor.FieldDescriptor(
      name='clusterer', full_name='proto_appstract.CreateClusteringRequest.clusterer', index=0,
      number=1, type=9, cpp_type=9, label=1,
      has_default_value=False, default_value=b"".decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='domain', full_name='proto_appstract.CreateClusteringRequest.domain', index=1,
      number=2, type=9, cpp_type=9, label=1,
      has_default_value=False, default_value=b"".decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='pages', full_name='proto_appstract.CreateClusteringRequest.pages', index=2,
      number=3, type=11, cpp_type=10, label=3,
      has_default_value=False, default_value=[],
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=152,
  serialized_end=250,
)


_CLUSTERINGRESPONSE = _descriptor.Descriptor(
  name='ClusteringResponse',
  full_name='proto_appstract.ClusteringResponse',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  create_key=_descriptor._internal_create_key,
  fields=[
    _descriptor.FieldDescriptor(
      name='clusters', full_name='proto_appstract.ClusteringResponse.clusters', index=0,
      number=1, type=11, cpp_type=10, label=3,
      has_default_value=False, default_value=[],
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='confidence', full_name='proto_appstract.ClusteringResponse.confidence', index=1,
      number=2, type=1, cpp_type=5, label=1,
      has_default_value=False, default_value=float(0),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=252,
  serialized_end=336,
)


_CLUSTERERSREPLY = _descriptor.Descriptor(
  name='ClusterersReply',
  full_name='proto_appstract.ClusterersReply',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  create_key=_descriptor._internal_create_key,
  fields=[
    _descriptor.FieldDescriptor(
      name='label', full_name='proto_appstract.ClusterersReply.label', index=0,
      number=1, type=9, cpp_type=9, label=1,
      has_default_value=False, default_value=b"".decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=338,
  serialized_end=370,
)


_EXPLOREREQUEST = _descriptor.Descriptor(
  name='ExploreRequest',
  full_name='proto_appstract.ExploreRequest',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  create_key=_descriptor._internal_create_key,
  fields=[
    _descriptor.FieldDescriptor(
      name='domain', full_name='proto_appstract.ExploreRequest.domain', index=0,
      number=1, type=9, cpp_type=9, label=1,
      has_default_value=False, default_value=b"".decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=372,
  serialized_end=404,
)


_EXPLOREREPLY = _descriptor.Descriptor(
  name='ExploreReply',
  full_name='proto_appstract.ExploreReply',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  create_key=_descriptor._internal_create_key,
  fields=[
    _descriptor.FieldDescriptor(
      name='content', full_name='proto_appstract.ExploreReply.content', index=0,
      number=1, type=9, cpp_type=9, label=1,
      has_default_value=False, default_value=b"".decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='url', full_name='proto_appstract.ExploreReply.url', index=1,
      number=2, type=9, cpp_type=9, label=1,
      has_default_value=False, default_value=b"".decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='domain', full_name='proto_appstract.ExploreReply.domain', index=2,
      number=3, type=9, cpp_type=9, label=1,
      has_default_value=False, default_value=b"".decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='origin', full_name='proto_appstract.ExploreReply.origin', index=3,
      number=4, type=9, cpp_type=9, label=1,
      has_default_value=False, default_value=b"".decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='nbNodes', full_name='proto_appstract.ExploreReply.nbNodes', index=4,
      number=5, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='nbLinks', full_name='proto_appstract.ExploreReply.nbLinks', index=5,
      number=6, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
    _descriptor.FieldDescriptor(
      name='error', full_name='proto_appstract.ExploreReply.error', index=6,
      number=7, type=8, cpp_type=7, label=1,
      has_default_value=False, default_value=False,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR,  create_key=_descriptor._internal_create_key),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=406,
  serialized_end=531,
)

_CLUSTER.fields_by_name['pages'].message_type = _PAGE
_CREATECLUSTERINGREQUEST.fields_by_name['pages'].message_type = _PAGE
_CLUSTERINGRESPONSE.fields_by_name['clusters'].message_type = _CLUSTER
DESCRIPTOR.message_types_by_name['Empty'] = _EMPTY
DESCRIPTOR.message_types_by_name['Page'] = _PAGE
DESCRIPTOR.message_types_by_name['Cluster'] = _CLUSTER
DESCRIPTOR.message_types_by_name['CreateClusteringRequest'] = _CREATECLUSTERINGREQUEST
DESCRIPTOR.message_types_by_name['ClusteringResponse'] = _CLUSTERINGRESPONSE
DESCRIPTOR.message_types_by_name['ClusterersReply'] = _CLUSTERERSREPLY
DESCRIPTOR.message_types_by_name['ExploreRequest'] = _EXPLOREREQUEST
DESCRIPTOR.message_types_by_name['ExploreReply'] = _EXPLOREREPLY
_sym_db.RegisterFileDescriptor(DESCRIPTOR)

Empty = _reflection.GeneratedProtocolMessageType('Empty', (_message.Message,), {
  'DESCRIPTOR' : _EMPTY,
  '__module__' : 'appstract_pb2'
  # @@protoc_insertion_point(class_scope:proto_appstract.Empty)
  })
_sym_db.RegisterMessage(Empty)

Page = _reflection.GeneratedProtocolMessageType('Page', (_message.Message,), {
  'DESCRIPTOR' : _PAGE,
  '__module__' : 'appstract_pb2'
  # @@protoc_insertion_point(class_scope:proto_appstract.Page)
  })
_sym_db.RegisterMessage(Page)

Cluster = _reflection.GeneratedProtocolMessageType('Cluster', (_message.Message,), {
  'DESCRIPTOR' : _CLUSTER,
  '__module__' : 'appstract_pb2'
  # @@protoc_insertion_point(class_scope:proto_appstract.Cluster)
  })
_sym_db.RegisterMessage(Cluster)

CreateClusteringRequest = _reflection.GeneratedProtocolMessageType('CreateClusteringRequest', (_message.Message,), {
  'DESCRIPTOR' : _CREATECLUSTERINGREQUEST,
  '__module__' : 'appstract_pb2'
  # @@protoc_insertion_point(class_scope:proto_appstract.CreateClusteringRequest)
  })
_sym_db.RegisterMessage(CreateClusteringRequest)

ClusteringResponse = _reflection.GeneratedProtocolMessageType('ClusteringResponse', (_message.Message,), {
  'DESCRIPTOR' : _CLUSTERINGRESPONSE,
  '__module__' : 'appstract_pb2'
  # @@protoc_insertion_point(class_scope:proto_appstract.ClusteringResponse)
  })
_sym_db.RegisterMessage(ClusteringResponse)

ClusterersReply = _reflection.GeneratedProtocolMessageType('ClusterersReply', (_message.Message,), {
  'DESCRIPTOR' : _CLUSTERERSREPLY,
  '__module__' : 'appstract_pb2'
  # @@protoc_insertion_point(class_scope:proto_appstract.ClusterersReply)
  })
_sym_db.RegisterMessage(ClusterersReply)

ExploreRequest = _reflection.GeneratedProtocolMessageType('ExploreRequest', (_message.Message,), {
  'DESCRIPTOR' : _EXPLOREREQUEST,
  '__module__' : 'appstract_pb2'
  # @@protoc_insertion_point(class_scope:proto_appstract.ExploreRequest)
  })
_sym_db.RegisterMessage(ExploreRequest)

ExploreReply = _reflection.GeneratedProtocolMessageType('ExploreReply', (_message.Message,), {
  'DESCRIPTOR' : _EXPLOREREPLY,
  '__module__' : 'appstract_pb2'
  # @@protoc_insertion_point(class_scope:proto_appstract.ExploreReply)
  })
_sym_db.RegisterMessage(ExploreReply)



_CLUSTERING = _descriptor.ServiceDescriptor(
  name='Clustering',
  full_name='proto_appstract.Clustering',
  file=DESCRIPTOR,
  index=0,
  serialized_options=None,
  create_key=_descriptor._internal_create_key,
  serialized_start=534,
  serialized_end=724,
  methods=[
  _descriptor.MethodDescriptor(
    name='CreateClustering',
    full_name='proto_appstract.Clustering.CreateClustering',
    index=0,
    containing_service=None,
    input_type=_CREATECLUSTERINGREQUEST,
    output_type=_CLUSTERINGRESPONSE,
    serialized_options=None,
    create_key=_descriptor._internal_create_key,
  ),
  _descriptor.MethodDescriptor(
    name='GetClusterers',
    full_name='proto_appstract.Clustering.GetClusterers',
    index=1,
    containing_service=None,
    input_type=_EMPTY,
    output_type=_CLUSTERERSREPLY,
    serialized_options=None,
    create_key=_descriptor._internal_create_key,
  ),
])
_sym_db.RegisterServiceDescriptor(_CLUSTERING)

DESCRIPTOR.services_by_name['Clustering'] = _CLUSTERING


_ROBOT = _descriptor.ServiceDescriptor(
  name='Robot',
  full_name='proto_appstract.Robot',
  file=DESCRIPTOR,
  index=1,
  serialized_options=None,
  create_key=_descriptor._internal_create_key,
  serialized_start=726,
  serialized_end=812,
  methods=[
  _descriptor.MethodDescriptor(
    name='Explore',
    full_name='proto_appstract.Robot.Explore',
    index=0,
    containing_service=None,
    input_type=_EXPLOREREQUEST,
    output_type=_EXPLOREREPLY,
    serialized_options=None,
    create_key=_descriptor._internal_create_key,
  ),
])
_sym_db.RegisterServiceDescriptor(_ROBOT)

DESCRIPTOR.services_by_name['Robot'] = _ROBOT

# @@protoc_insertion_point(module_scope)
