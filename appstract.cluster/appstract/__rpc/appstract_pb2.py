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
  serialized_pb=b'\n\x0f\x61ppstract.proto\x12\x0fproto_appstract\"\x07\n\x05\x45mpty\"\x1b\n\x08Progress\x12\x0f\n\x07message\x18\x01 \x01(\t\"$\n\x04Page\x12\x0f\n\x07\x63ontent\x18\x01 \x01(\t\x12\x0b\n\x03url\x18\x02 \x01(\t\"?\n\x17\x43reateClusteringRequest\x12$\n\x05pages\x18\x03 \x03(\x0b\x32\x15.proto_appstract.Page\" \n\x0e\x45xploreRequest\x12\x0e\n\x06\x64omain\x18\x01 \x01(\t\"}\n\x0c\x45xploreReply\x12\x0f\n\x07\x63ontent\x18\x01 \x01(\t\x12\x0b\n\x03url\x18\x02 \x01(\t\x12\x0e\n\x06\x64omain\x18\x03 \x01(\t\x12\x0e\n\x06origin\x18\x04 \x01(\t\x12\x0f\n\x07nbNodes\x18\x05 \x01(\x05\x12\x0f\n\x07nbLinks\x18\x06 \x01(\x05\x12\r\n\x05\x65rror\x18\x07 \x01(\x08\x32V\n\nClustering\x12H\n\x0fStartClustering\x12\x16.proto_appstract.Empty\x1a\x19.proto_appstract.Progress\"\x00\x30\x01\x32V\n\x05Robot\x12M\n\x07\x45xplore\x12\x1f.proto_appstract.ExploreRequest\x1a\x1d.proto_appstract.ExploreReply\"\x00\x30\x01\x62\x06proto3'
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


_PROGRESS = _descriptor.Descriptor(
  name='Progress',
  full_name='proto_appstract.Progress',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  create_key=_descriptor._internal_create_key,
  fields=[
    _descriptor.FieldDescriptor(
      name='message', full_name='proto_appstract.Progress.message', index=0,
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
  serialized_start=45,
  serialized_end=72,
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
  serialized_start=74,
  serialized_end=110,
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
      name='pages', full_name='proto_appstract.CreateClusteringRequest.pages', index=0,
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
  serialized_start=112,
  serialized_end=175,
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
  serialized_start=177,
  serialized_end=209,
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
  serialized_start=211,
  serialized_end=336,
)

_CREATECLUSTERINGREQUEST.fields_by_name['pages'].message_type = _PAGE
DESCRIPTOR.message_types_by_name['Empty'] = _EMPTY
DESCRIPTOR.message_types_by_name['Progress'] = _PROGRESS
DESCRIPTOR.message_types_by_name['Page'] = _PAGE
DESCRIPTOR.message_types_by_name['CreateClusteringRequest'] = _CREATECLUSTERINGREQUEST
DESCRIPTOR.message_types_by_name['ExploreRequest'] = _EXPLOREREQUEST
DESCRIPTOR.message_types_by_name['ExploreReply'] = _EXPLOREREPLY
_sym_db.RegisterFileDescriptor(DESCRIPTOR)

Empty = _reflection.GeneratedProtocolMessageType('Empty', (_message.Message,), {
  'DESCRIPTOR' : _EMPTY,
  '__module__' : 'appstract_pb2'
  # @@protoc_insertion_point(class_scope:proto_appstract.Empty)
  })
_sym_db.RegisterMessage(Empty)

Progress = _reflection.GeneratedProtocolMessageType('Progress', (_message.Message,), {
  'DESCRIPTOR' : _PROGRESS,
  '__module__' : 'appstract_pb2'
  # @@protoc_insertion_point(class_scope:proto_appstract.Progress)
  })
_sym_db.RegisterMessage(Progress)

Page = _reflection.GeneratedProtocolMessageType('Page', (_message.Message,), {
  'DESCRIPTOR' : _PAGE,
  '__module__' : 'appstract_pb2'
  # @@protoc_insertion_point(class_scope:proto_appstract.Page)
  })
_sym_db.RegisterMessage(Page)

CreateClusteringRequest = _reflection.GeneratedProtocolMessageType('CreateClusteringRequest', (_message.Message,), {
  'DESCRIPTOR' : _CREATECLUSTERINGREQUEST,
  '__module__' : 'appstract_pb2'
  # @@protoc_insertion_point(class_scope:proto_appstract.CreateClusteringRequest)
  })
_sym_db.RegisterMessage(CreateClusteringRequest)

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
  serialized_start=338,
  serialized_end=424,
  methods=[
  _descriptor.MethodDescriptor(
    name='StartClustering',
    full_name='proto_appstract.Clustering.StartClustering',
    index=0,
    containing_service=None,
    input_type=_EMPTY,
    output_type=_PROGRESS,
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
  serialized_start=426,
  serialized_end=512,
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
